using System;
using System.Collections.Generic;

namespace Server.DataAccess.Models
{
    public class Play
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PlayerId { get; set; }

        public int ChosenNumbersCount { get; set; }

        public decimal PriceDKK => Core.Game.ChosenNumbersPackages.GetPrice(ChosenNumbersCount);

        public List<int> ChosenNumbers { get; set; } = [];

        public Guid? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PaidAt { get; set; }
    }
}
