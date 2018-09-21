using System;
using QTFK.Attributes;
using QTFK.Services;

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