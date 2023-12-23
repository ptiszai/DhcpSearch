
namespace DhcpSearch
{
    public enum StatusEnum : int
    {
        NONE = 0,
        // ping
        PING_START = 1,
        PING_RUNNING = 2,
        PING_TIMEOUT_ERROR = 3,
        PING_GENERAL_ERROR = 4,
        PING_SUCCESS = 5,
        PING_FINISHED = 6,
        GENERAL_ERROR = 7,
        PING_WAIT = 8,
        // Ingenico.css
        DHCP_EXECUTOR = 20,
        PAYMENTIN = 21,
    }
}
