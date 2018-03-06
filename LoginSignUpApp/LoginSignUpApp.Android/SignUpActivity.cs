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
using System.Text.RegularExpressions;
using System.Globalization;

namespace LoginSignUpApp.Droid
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {

        bool invalid = false;
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
                Android.App.AlertDialog.Builder message = new Android.App.AlertDialog.Builder(this);
                string name = uName.Text;
                string pin = pWord.Text;
                string rePin = rePWord.Text;
                string em = email.Text;
                string encoded="";

                int responseString;

                if ((name != "") && (pin != "") && (rePin != "") && (em != ""))
                {
                    if (pin.Equals(rePin))
                    {
                        if (IsValidEmail(em))
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

                                var response = await RunPostAsync(name, encoded, em);
                                responseString = int.Parse(response);

                                //Console.WriteLine(response);

                                if(responseString == 1)
                                {
                                    message.SetTitle("Successful!!");
                                    message.SetMessage("You have resiger successfully!");
                                    message.SetNegativeButton("OK", (c, ev) => 
                                    {
                                        Intent intent = new Intent(this, typeof(MainActivity));
                                        this.StartActivity(intent);
                                    });
                                    message.Show();
                                }
                                if(responseString == 2)
                                {
                                    message.SetTitle("Opssss");
                                    message.SetMessage("The Email address is exist, try another one...");
                                    message.SetNegativeButton("OK", (c, ev) => { });
                                    message.Show();
                                }
                                if(responseString == 0)
                                {
                                    message.SetTitle("Opssss");
                                    message.SetMessage("The username is exist, try another one...");
                                    message.SetNegativeButton("OK", (c, ev) => { });
                                    message.Show();
                                }

                            }
                            catch
                            {
                                message.SetTitle("Opssss");
                                message.SetMessage("Some internal error, try again later..");
                                message.SetNegativeButton("OK", (c, ev) => { });
                                message.Show();
                            }
                        }
                        else
                        {
                            message.SetTitle("Opssss");
                            message.SetMessage("Invalide Email address..");
                            message.SetNegativeButton("OK", (c, ev) => { });
                            message.Show();
                        }
                    }
                    else
                    {
                        message.SetTitle("Opssss");
                        message.SetMessage("The password doesn't match..");
                        message.SetNegativeButton("OK", (c, ev) => { });
                        message.Show();
                    }
                    
                }
                else
                {
                    message.SetTitle("Opssss");
                    message.SetMessage("Some value are missing..");
                    message.SetNegativeButton("OK", (c, ev) => { });
                    message.Show();
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

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}