namespace UI
{
    public class DebugTypeAndID
    {
        public DebugTypeAndID(int instanceID, EDebugType debugType)
        {
            InstanceID = instanceID;
            DebugType = debugType;
        }
        
        public EDebugType DebugType { get; set; }
        public int InstanceID { get; set; }
    }
}