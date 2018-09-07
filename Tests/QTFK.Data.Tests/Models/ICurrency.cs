using QTFK.Attributes;
using QTFK.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Models
{
    public interface ICurrency : IEntity
    {
        [Id]
        int Id { get; set; }
        string Name { get; set; }

        [Foreign("fromCurrencyId")]
        IView<ICurrencyConversion> Exchanges { get; }
    }
}