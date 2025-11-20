using System;
using DataAccess.Pricing;
using DataAccess.Models;

namespace DataAccess.Models;

public class Board
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PlayerId { get; set; }
    public Player Player { get; set; }

    public int ChosenNumbersCount { get; set; }

    public decimal PriceDKK => ChosenNumbersPackages.GetPrice(ChosenNumbersCount);
    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; }
}