using System.ComponentModel.DataAnnotations.Schema;

namespace ResidenceMngSys.Models
{
    public class WalletTransactions : ITenantEntity
    {
        [Column("İd")]
        public int Id { get; set; }
        [Column("Dues_İd")]
        public int? Dues_Id {  get; set; }
        [Column("Wallet_İd")]
        public int Wallet_Id { get; set; }
        public decimal Amount {  get; set; }
        public string TransactionType { get; set; }
        public string? Description {  get; set; }
        public DateTime CreatedAt {  get; set; }
        public decimal BalanceAfter {  get; set; }


        [ForeignKey("Wallet_Id")]
        public Wallet wallet { get; set; }
        [ForeignKey("Dues_Id")]
        public Dues dues { get; set; }

        public int TenantId { get; set; }
        public Tenants Tenant { get; set; } // navigation property
    }
}
