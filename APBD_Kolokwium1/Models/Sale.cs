using System;
using System.Collections.Generic;

namespace APBD_Kolokwium1.Models;

public partial class Sale
{
    public int IdSale { get; set; }

    public int IdClient { get; set; }

    public int IdSubscription { get; set; }

    public DateOnly CreatedAt { get; set; }

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Subscription IdSubscriptionNavigation { get; set; } = null!;
}
