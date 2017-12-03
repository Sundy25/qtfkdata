using QTFK.Attributes;

namespace SampleLibrary.Models
{
    [Alias("person")]
    public class SampleClass
    {
        [Key]
        public int? ID { get; set; }

        public string Name { get; set; }

        [Alias("wallet_cash")]
        public decimal WalletCash { get; set; }
    }
}
