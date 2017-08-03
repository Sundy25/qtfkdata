using QTFK.Attributes;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Data.Tests.Models
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
