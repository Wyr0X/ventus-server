using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace VentusServer
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(FirebaseService firebaseService)
        {
            if (firebaseService.App == null)
                throw new InvalidOperationException("FirebaseApp no está inicializado correctamente.");

            _firestoreDb = FirestoreDb.Create("ventus-ao");
        }

        public FirestoreDb GetFirestoreDb() => _firestoreDb;

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            var accountsRef = _firestoreDb.Collection("accounts");
            var query = accountsRef.WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return null;
            }

            var document = snapshot.Documents[0];
            return document.ConvertTo<AccountModel>();
        }

        public async Task SaveAccountAsync(AccountModel account)
        {
            var accountsRef = _firestoreDb.Collection("accounts");
            var documentRef = accountsRef.Document(account.Email);
            await documentRef.SetAsync(account);
        }
    }
}
