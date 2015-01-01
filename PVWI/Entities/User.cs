// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace PVWI.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Entidade com todos os campos que um usuário irá ter.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id do usuário para referência no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do usuário obrigatório no banco de dados.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// E-mail do usuário obrigatório no banco de dados e valida como um formato de e-mail.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}