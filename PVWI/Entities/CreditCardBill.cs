// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardBill.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Entities
{
    using System;

    /// <summary>
    /// The credit card bill.
    /// </summary>
    public class CreditCardBill
    {
        /// <summary>
        /// Id da fatura no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dia de Pagamento da fatura.
        /// </summary>
        public DateTime BillPaymentDateTime { get; set; }
    }
}