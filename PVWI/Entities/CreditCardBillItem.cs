// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardBillItem.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace PVWI.Entities
{
    using System;

    /// <summary>
    /// Define os campos que um item de uma fatura deve ter.
    /// </summary>
    public class CreditCardBillItem
    {
        /// <summary>
        /// Id para pesquisa no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição que o item possui na fatura.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data que o item possui na fatura.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Valor em reais que o item possui na fatura.
        /// </summary>
        public double ItemValue { get; set; }

        /// <summary>
        /// Id do usuário que é responsável por pagar aquele item na fatura.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Objeto do tipo usuário que é responsável por pagar aquele item na fatura.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Id da fatura que o item pertence.
        /// </summary>
        public int CreditCardBillId { get; set; }

        /// <summary>
        /// Objeto do tipo fatura que o item pertence.
        /// </summary>
        public CreditCardBill CreditCardBill { get; set; }
    }
}