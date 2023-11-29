using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PempoPaymentGateway.Models
{
    public class PaymentRequestDto
    {
        string _cardNumber;
        string _expirationDate;
        string _cvv;
        decimal _amount;
        public PaymentRequestDto(string cardNumber, string expirationDate, string cvv, decimal amount)
        {
            _cardNumber = cardNumber;   
            _expirationDate = expirationDate;
            _cvv = cvv;
            _amount = amount;   
        }
    }
}