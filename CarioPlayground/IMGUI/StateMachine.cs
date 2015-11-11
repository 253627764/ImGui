namespace IMGUI
{
    public class StateMachine
    {
        class StateTransition
        {
            readonly string CurrentState;
            readonly string Command;

            public StateTransition(string currentState, string command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        System.Collections.Generic.Dictionary<StateTransition, string> transitions;
        /// <summary>
        /// Current state of the state machine
        /// </summary>
        /// <remarks>DO NOT Set this property if you are not making a instant state transition!</remarks>
        public string CurrentState { get; set; }

        public StateMachine(string initialState, string[] stateTransitions)
        {
            CurrentState = initialState;
            transitions = new System.Collections.Generic.Dictionary<StateTransition, string>();
            for (int i = 0; i < stateTransitions.Length; i+=3)
            {
                transitions.Add(new StateTransition(stateTransitions[i], stateTransitions[i+1]), stateTransitions[i+2]);
            }
        }

        public string GetNext(string command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            string nextState;
            if(!transitions.TryGetValue(transition, out nextState))
            {
                //throw new System.Exception("Invalid transition: " + CurrentState + " -> " + command);
                return CurrentState;
            }
            return nextState;
        }

        /// <summary>
        /// ���������ƶ�״̬
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true:״̬�����仯/false:״̬û�б仯</returns>
        /// <remarks>����״̬�����仯ʱ��������Чʱ��������µı仯Ϊ״̬ͼ�е�һ�������ʱ��ʹ��Fetch���</remarks>
        public bool MoveNext(string command)
        {
            var oldState = CurrentState;
            CurrentState = GetNext(command);
            return CurrentState != oldState;
        }
    }
}