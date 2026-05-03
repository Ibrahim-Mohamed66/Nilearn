using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common
{
    public class PaymentSessionResult
    {
        public string Provider { get; set; }
        public string PaymentUrl { get; set; }

        public string PaymobIntentionId { get; set; } = null!;
        public string PaymobOrderId { get; set; } = null!;
    }
}
