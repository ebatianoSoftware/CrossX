using CrossX.Forms.Transitions;

namespace CrossX.Forms
{
    public interface ITransitionsManager
    {
        void LoadTransitions(string path);

        StateTransition CreateStateTransition(string key, string name);
        Transition CreateTransition(string key, string name);
    }
}
