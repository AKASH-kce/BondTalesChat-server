namespace BondTalesChat_Server.Interfaces
{
    public interface IMessageDelivery
    {
        int MessageId { get; set; }
        int UserId { get; set; }
        byte Status { get; set; } // 0=Sent,1=Delivered,2=Read
        DateTime? DeliveredAt { get; set; }
        DateTime? ReadAt { get; set; }
    }

}
