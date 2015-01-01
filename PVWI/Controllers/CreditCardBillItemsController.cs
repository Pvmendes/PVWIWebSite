// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardBillItemsController.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Controllers
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;

    using PVWI.DAO;
    using PVWI.Entities;

    /// <summary>
    /// O Controlador para items de faturas.
    /// </summary>
    public class CreditCardBillItemsController : ApiController
    {
        /// <summary>
        /// The db.
        /// </summary>
        private readonly PvwiContext db = new PvwiContext();

        // GET: api/CreditCardBillItems

        /// <summary>
        /// Retorna todas os items de uma fatura.
        /// </summary>
        /// <returns>
        /// O retorno do tipo <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<CreditCardBillItem> GetBillItems()
        {
            return this.db.BillItems;
        }

        // GET: api/CreditCardBillItems/5

        /// <summary>
        /// Retorna o item especificado de uma fatura.
        /// </summary>
        /// <param name="id">
        /// Id do item de uma fatura.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(CreditCardBillItem))]
        public IHttpActionResult GetCreditCardBillItem(int id)
        {
            var creditCardBillItem = this.db.BillItems.Find(id);
            if (creditCardBillItem == null)
            {
                return this.NotFound();
            }

            return this.Ok(creditCardBillItem);
        }

        // PUT: api/CreditCardBillItems/5

        /// <summary>
        /// Coloca dados dentro de um item de uma fatura.
        /// </summary>
        /// <param name="id">
        /// Id do item de uma fatura.
        /// </param>
        /// <param name="creditCardBillItem">
        /// O item que sera adicionado dados.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCreditCardBillItem(int id, CreditCardBillItem creditCardBillItem)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != creditCardBillItem.Id)
            {
                return this.BadRequest();
            }

            this.db.Entry(creditCardBillItem).State = EntityState.Modified;

            try
            {
                this.db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.CreditCardBillItemExists(id))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CreditCardBillItems

        /// <summary>
        /// Adiciona um novo item de uma fatura.
        /// </summary>
        /// <param name="creditCardBillItem">
        /// O item a ser adicionado.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(CreditCardBillItem))]
        public IHttpActionResult PostCreditCardBillItem(CreditCardBillItem creditCardBillItem)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.db.BillItems.Add(creditCardBillItem);
            this.db.SaveChanges();

            return this.CreatedAtRoute("DefaultApi", new { id = creditCardBillItem.Id }, creditCardBillItem);
        }

        // DELETE: api/CreditCardBillItems/5

        /// <summary>
        /// Deleta um item na fatura com o id especifico.
        /// </summary>
        /// <param name="id">
        /// Id do item de uma fatura.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(CreditCardBillItem))]
        public IHttpActionResult DeleteCreditCardBillItem(int id)
        {
            var creditCardBillItem = this.db.BillItems.Find(id);
            if (creditCardBillItem == null)
            {
                return this.NotFound();
            }

            this.db.BillItems.Remove(creditCardBillItem);
            this.db.SaveChanges();

            return this.Ok(creditCardBillItem);
        }

        /// <summary>
        /// Desfaz de um contexto com alteracoes no banco de dados.
        /// </summary>
        /// <param name="disposing">
        /// Indica se esta desfazendo ou nao do contexto do banco de dados.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Encontra um item de fatura dentro do banco de dados.
        /// </summary>
        /// <param name="id">
        /// Id do item de uma fatura.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="bool"/>.
        /// </returns>
        private bool CreditCardBillItemExists(int id)
        {
            return this.db.BillItems.Count(e => e.Id == id) > 0;
        }
    }
}