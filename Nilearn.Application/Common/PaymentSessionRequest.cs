using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common
{
    public class PaymentSessionRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MerchantReferenceId { get; set; }
        public string CourseTitle { get; set; }
        public decimal CoursePrice { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int EnrollmentId { get; set; }
        public string PhoneNumber { get; set; }

    }
}
