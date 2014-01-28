

namespace FrameWork
{
    // Niveau d'isolation des Transactions
    public enum IsolationLevel
    {
        DEFAULT,
        SERIALIZABLE,
        REPEATABLE_READ,
        READ_COMMITTED,
        READ_UNCOMMITTED,
        SNAPSHOT
    }
}