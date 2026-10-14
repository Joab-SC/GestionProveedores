using System;
using System.Security.Cryptography;
using System.Text;

namespace Proveedores.Negocio
{
    // Hash SHA-256 de la contrasena, en hexadecimal en minuscula
    // (64 caracteres), igual que la columna usuario.contrasena.
    // Es interno: solo lo usan los servicios de esta capa (CU-01 y CU-11).
    internal static class HashContrasena
    {
        public static string Calcular(string contrasena)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(contrasena);
            return Convert.ToHexStringLower(SHA256.HashData(bytes));
        }
    }
}
