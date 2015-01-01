// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersController.cs" company="PVWI Family">
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
    /// The users controller.
    /// </summary>
    public class UsersController : ApiController
    {
        /// <summary>
        /// The db.
        /// </summary>
        private readonly PvwiContext db = new PvwiContext();

        // GET: api/Users

        /// <summary>
        /// Retorna todos os usuários cadastrados no sistema.
        /// </summary>
        /// <returns>
        /// Tipo de retorno <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<User> GetUsers()
        {
            return this.db.Users;
        }

        // GET: api/Users/5

        /// <summary>
        /// Retorna o usuário específicado pela Id.
        /// </summary>
        /// <param name="id">
        /// A Id de identificação do usuário na tabela do Banco de Dados.
        /// </param>
        /// <returns>
        /// O tipo de retorno <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            var user = this.db.Users.Find(id);
            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }

        // PUT: api/Users/5

        /// <summary>
        /// Coloca os dados de um usuário específico.
        /// </summary>
        /// <param name="id">
        /// A Id de identificação do usuário na tabela do Banco de Dados.
        /// </param>
        /// <param name="user">
        /// O objeto do tipo entidade usuário.
        /// </param>
        /// <returns>
        /// O tipo de retorno <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != user.Id)
            {
                return this.BadRequest();
            }

            this.db.Entry(user).State = EntityState.Modified;

            try
            {
                this.db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.UserExists(id))
                {
                    return this.NotFound();
                }

                throw;
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users

        /// <summary>
        /// Adiciona um usuário.
        /// </summary>
        /// <param name="user">
        /// O usuário pronto para adicionar no banco de dados.
        /// </param>
        /// <returns>
        /// O tipo de retorno <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.db.Users.Add(user);
            this.db.SaveChanges();

            return this.CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5

        /// <summary>
        /// Deleta um usuário específicado pela Id.
        /// </summary>
        /// <param name="id">
        /// A Id de identificação do usuário na tabela do Banco de Dados.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            var user = this.db.Users.Find(id);
            if (user == null)
            {
                return this.NotFound();
            }

            this.db.Users.Remove(user);
            this.db.SaveChanges();

            return this.Ok(user);
        }

        /// <summary>
        /// Se defaz do contexto do banco de dados.
        /// </summary>
        /// <param name="disposing">
        /// Ação de verdadeiro ou falso se está sendo desfeita ou não dos dados do Contexto para o banco de dados.
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
        /// Checa se o usuário específicado pela Id existe.
        /// </summary>
        /// <param name="id">
        /// A Id de identificação do usuário na tabela do Banco de Dados.
        /// </param>
        /// <returns>
        /// O retorno do tipo <see cref="bool"/>.
        /// </returns>
        private bool UserExists(int id)
        {
            return this.db.Users.Count(e => e.Id == id) > 0;
        }
    }
}