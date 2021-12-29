using UnityEngine;

namespace TopDownCharacter
{
    public class CharacterBehaviour : MonoBehaviour
    {

        protected Character Character;
        [SerializeField] bool _loggingEnabled;
        bool LoggingEnabled => _loggingEnabled;

        protected virtual void Awake()
        {
            Character = transform.root.GetComponentInChildren<Character>();
        }
        
        protected void Log(string logString)
        {
            if (!LoggingEnabled) return;
            
            string logger = this.GetType().Name;
            Debug.Log($"{logger}: {logString}");
        }

        protected void Draw(string name, bool val)
        {
            if (!LoggingEnabled) return;
            
            string logger = this.GetType().Name + "." + name;
            DebugGraph.Log(name, val);
        }

        protected void Trace(string name, Vector2 vector)
        {
            if (!LoggingEnabled) return;
            
            string logger = this.GetType().Name + "." + name;
            DebugGraph.Draw(name, vector);
        }
        protected void Draw(string name, float f)
        {
            if (!LoggingEnabled) return;
            
            string logger = this.GetType().Name + "." + name;
            DebugGraph.Log(name, f);
        }
    }
}