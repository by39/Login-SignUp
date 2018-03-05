using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace LoginSignUpApp.Droid
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUpPage);
            // Create your application here

            EditText uName = FindViewById<EditText>(Resource.Id.uName);
            EditText pWord = FindViewById<EditText>(Resource.Id.pWord);
            EditText rePWord = FindViewById<EditText>(Resource.Id.rePWord);
            EditText email = FindViewById<EditText>(Resource.Id.email);

            Button signUp = FindViewById<Button>(Resource.Id.signUp);
  
            signUp.Click += async (s,e) =>
            {
                string name = uName.Text;
                string pin = pWord.Text;
                string rePin = rePWord.Text;
                string em = email.Text;
                string encoded="";

                if ((name != "") && (pin != "") && (rePin != "") && (em != ""))
                {
                    if (pin.Equals(rePin))
                    {
                        try
                        {
                            string orig = pin;

                            byte[] salt;
                            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                            var pbkdf2 = new Rfc2898DeriveBytes(orig, salt, 10000);

                            byte[] hash = pbkdf2.GetBytes(20);
                            byte[] hashBytes = new byte[36];

                            Array.Copy(salt, 0, hashBytes, 0, 16);
                            Array.Copy(hash, 0, hashBytes, 16, 20);

                            encoded = Convert.ToBase64String(hashBytes);

                            Console.WriteLine(encoded);
                            
                        }
                        catch
                        {
                            Console.WriteLine("Encode errors.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The password doesn't match");
                    }
                    var response = await RunPostAsync(name, encoded, em);
                    
                }
                else
                {
                    Console.WriteLine("Some value are missing");
                }

                
            };
        }

        public async Task<string> RunPostAsync(string name, string pword, string email)
        {

            try
            {
                HttpClient client = new HttpClient();
                var RestUrl = "http://10.20.1.68:45455/api/Person/";
                var request = new HttpRequestMessage(HttpMethod.Post, RestUrl);

                var p = new Person();

                p.name = name;
                p.password = pword;
                p.email = email;

                var json = JsonConvert.SerializeObject(p);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}