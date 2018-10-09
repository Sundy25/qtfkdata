using System;
using QTFK.Data.Attributes;

namespace QTFK.Data.Tests.Models
{
    public interface ICurrencyConversion : IEntity
    {
        decimal Value { get; set; }
        DateTime Date { get; set; }

        [Id]
        [Column("fromCurrencyId")]
        ICurrency From { get; set; }

        [Id]
        [Column("toCurrencyId")]
        ICurrency To { get; set; }
    }
}