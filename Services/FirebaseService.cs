using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Threading.Tasks;

namespace VentusServer
{
    public class FirebaseService
    {
        public FirebaseApp App { get; }

        // Constructor para inicializar FirebaseApp con las credenciales
        public FirebaseService(string credentialsPath)
        {
            try
            {
                // Si FirebaseApp ya existe, se maneja la excepción para evitar la creación repetida
                if (FirebaseApp.DefaultInstance == null)
                {
                    App = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(credentialsPath)
                    });
                }
                else
                {
                    // Si la app ya existe, simplemente la asignamos
                    App = FirebaseApp.DefaultInstance;
                }
            }
            catch (InvalidOperationException ex)
            {
                // Manejo de excepciones si no se puede crear una nueva instancia
                throw new Exception("FirebaseApp could not be created or accessed.", ex);
            }
        }

        // Método para verificar el token de Google
        public async Task<FirebaseToken> VerifyTokenAsync(string idToken)
        {
            try
            {
                // Verificar el id_token de Google y devolver el FirebaseToken
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken; // Devolvemos el token decodificado con la información del usuario
            }
            catch (Exception ex)
            {
                throw new Exception("Token verification failed", ex); // Manejo de errores más específico
            }
        }
    }
}
