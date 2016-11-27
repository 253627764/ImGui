﻿using System;
using System.Collections.Generic;

namespace ImGui
{
    public abstract class Form
    {
        public static Form current;
        IWindow window;
        internal DrawList DrawList = new DrawList();
        internal IRenderer renderer;

        internal LayoutCache layoutCache = new LayoutCache();

        internal GUIState uiState = new GUIState();

        protected Form(Rect rect):this(rect.TopLeft, rect.Size)
        {
        }

        protected Form(Point position, Size size, string Title = "<unnamed>")
        {
            this.window = Application.windowContext.CreateWindow(position, size);
            this.window.Title = Title;

            renderer = Application._map.CreateRenderer();
            renderer.Init(this.Pointer);
            
            this.DrawList.DrawBuffer.CommandBuffer.Add(new DrawCommand());
            this.DrawList.BezierBuffer.CommandBuffer.Add(new DrawCommand());

            InitGUI();
        }

        public FormState FormState
        {
            get { return formState; }
            set { formState = value; }
        }
        
        internal LayoutCache LayoutCache
        {
            get { return layoutCache; }
        }

        #region window management

        public bool Closed { get; private set; }

        public IntPtr Pointer { get { return window.Pointer; } }

        public Size Size
        {
            get { return window.Size; }
            set { window.Size = value; }
        }

        public Point Position
        {
            get { return window.Position; }
            set { window.Position = value; }
        }

        public bool Focused { get { throw new NotImplementedException(); } }

        public void Show()
        {
            window.Show();
            Event.current.type = EventType.Layout;
        }

        public void Hide()
        {
            window.Hide();
        }

        public void Close()
        {
            this.renderer.ShutDown();

            window.Close();
            this.Closed = true;
        }

        public void Minimize()
        {
            Event.current.type = EventType.MinimizeWindow;
        }

        public void Maximize()
        {
            Event.current.type = EventType.MaximizeWindow;
        }

        public void Normalize()
        {
            Event.current.type = EventType.NormalizeWindow;

#if implementation
            var originalWindowRect = new Rect(window.NormalPosition, window.NormalSize);
#endif
        }

        #endregion

        #region the GUI Loop

        internal static void BeginGUI(bool useGUILayout)
        {
            if (useGUILayout)
            {
                LayoutUtility.Begin();
            }

            // Clear reference to active widget if the widget isn't alive anymore
            var g = Form.current.uiState;
            g.HoverIdPreviousFrame = g.HoverId;
            g.HoverId = GUIState.None;
            if (!g.ActiveIdIsAlive && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != GUIState.None)
                g.ActiveId = GUIState.None;
            g.ActiveIdPreviousFrame = g.ActiveId;
            g.ActiveIdIsAlive = false;
            g.ActiveIdIsJustActivated = false;
        }

        /// <summary>
        /// Custom GUI Logic. This should be implemented by the user.
        /// </summary>
        protected abstract void OnGUI();

        internal static void EndGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                LayoutUtility.Layout();
                Event.current.type = EventType.Repaint;
            }
        }

        private long lastFPSUpdateTime;
        private int fps;
        private int elapsedFrameCount = 0;

        /// <summary>
        /// GUI Loop
        /// </summary>
        internal void GUILoop()
        {
            current = this;

            handleEvent();

            elapsedFrameCount++;
            var detlaTime = Application.Time - lastFPSUpdateTime;
            if (detlaTime > 1000)
            {
                fps = elapsedFrameCount;
                elapsedFrameCount = 0;
                lastFPSUpdateTime = Application.Time;
            }

            Console.Clear();
            Console.WriteLine("{0,5:0.0}, {1}", fps, this.GetMousePos().ToString());
            Console.WriteLine("ActiveId: {0}, ActiveIdIsAlive: {1}", this.uiState.ActiveId, this.uiState.ActiveIdIsAlive);
            Console.WriteLine("HoverId: {0}", this.uiState.HoverId);
        }

        void handleEvent()
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                    LayoutUtility.current.Clear();
                    LoadFormGroup();
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    break;
                case EventType.Repaint:
                    if (!this.Closed)
                    {
                        this.DrawList.Clear();
                        BeginGUI(true);
                        OnGUI();
                        EndGUI();
                        Render();
                        //Event.current.type = EventType.Used;
                    }
                    break;
                case EventType.MaximizeWindow:
                    {
                        window.Maximize();
                        FormState = FormState.Maximized;

                        Event.current.type = EventType.Layout;
                    }
                    break;
                case EventType.MinimizeWindow:
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    window.Minimize();
                    FormState = FormState.Minimized;
                    Event.current.type = EventType.Used;
                    break;
                case EventType.NormalizeWindow:
                    {
                        window.Normalize();
                        FormState = FormState.Normal;

                        Event.current.type = EventType.Layout;
                    }
                    break;
                default:
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    break;
            }
        }

        void Render()
        {
            this.renderer.Clear();
            this.renderer.RenderDrawList(this.DrawList, (int)this.Size.Width, (int)this.Size.Height);
            this.renderer.SwapBuffers();
        }

        private FormState formState = FormState.Normal;

        /// <summary>
        /// Call this to initialize GUI
        /// </summary>
        private void InitGUI()
        {
            var clientWidth = (int) Size.Width;
            var clientHeight = (int) Size.Height;
            
            // init the layout group of this form
            LoadFormGroup();

            // init the event
            Event.current = new Event();
        }

        private void LoadFormGroup()
        {
            var formGroup = new LayoutGroup(true, Style.Default, GUILayout.Width(this.Size.Width),
                GUILayout.Height(this.Size.Height));
            formGroup.isForm = true;
            layoutCache.Push(formGroup);
        }

#endregion

        /// <summary>
        /// Get the mouse position relative to this form
        /// </summary>
        public Point GetMousePos()
        {
            return Application.windowContext.ScreenToClient(window, Application.inputContext.MousePosition);
        }

        public Point ScreenToClient(Point point)
        {
            return window.ScreenToClient(point);
        }

        public Point ClientToScreen(Point point)
        {
            return window.ClientToScreen(point);
        }
    }
}