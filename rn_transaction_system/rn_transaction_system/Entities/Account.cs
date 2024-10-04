namespace rn_transaction_system.Entities
{
    public class Account
    {
        public int Id { get; set; } 

        public decimal Balance { get; set; } 

        public string UserId { get; set; } 

        public byte[] RowVersion { get; set; } 
    }
}
