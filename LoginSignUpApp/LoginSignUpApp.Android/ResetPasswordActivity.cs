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
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace LoginSignUpApp.Droid
{
    [Activity(Label = "ResetPasswordActivity")]
    public class ResetPasswordActivity : Activity
    {
        HttpClient client = new HttpClient();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ResetPassword);

            EditText uname = FindViewById<EditText>(Resource.Id.uname);
            EditText email = FindViewById<EditText>(Resource.Id.email);
            Button reset = FindViewById<Button>(Resource.Id.resetBtn);

            LinearLayout pWordLayout = FindViewById<LinearLayout>(Resource.Id.reSetPwordLayout);

            EditText newPword = FindViewById<EditText>(Resource.Id.newPword);
            EditText reNewpword = FindViewById<EditText>(Resource.Id.reNewPword);
            Button done = FindViewById<Button>(Resource.Id.doneBtn);
            reset.Click += async (s,e)=>
            {
                Android.App.AlertDialog.Builder message = new Android.App.AlertDialog.Builder(this);

                string uName = uname.Text;
                string eMail = email.Text;
                //string encoded = "";
                var response = await RunGetAsync(uName);
                if(response != null)
                {
                    Person p = new Person();

                    p.id = response.id;
                    p.name = response.name;
                    p.password = response.password;
                    p.email = response.email;
                    //bool eq = eMail.Equals(p.email);
                    if (eMail.Equals(p.email))
                    {
                        pWordLayout.Visibility = ViewStates.Visible;

                    }
                    else
                    {
                        message.SetTitle("Opssss");
                        message.SetMessage("The Email incorrect");
                        message.SetNegativeButton("OK", (c, ev) => { });
                        message.Show();
                    }
                }
                else
                {
                    message.SetTitle("Opssss");
                    message.SetMessage("The user not exist..");
                    message.SetNegativeButton("OK", (c, ev) => { });
                    message.Show();
                }
                
                //Console.WriteLine(eq);
            };

            done.Click += async (s,e) =>
            {
                Android.App.AlertDialog.Builder message = new Android.App.AlertDialog.Builder(this);
                string uName = uname.Text;
                string newpword = newPword.Text;
                string renewpword = reNewpword.Text;
                string eMail = email.Text;
                string encoded = "";

                if (newpword.Equals(renewpword))
                {
                    string orig = newpword;

                    byte[] salt;
                    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                    var pbkdf2 = new Rfc2898DeriveBytes(orig, salt, 10000);

                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashBytes = new byte[36];

                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);

                    encoded = Convert.ToBase64String(hashBytes);

                    Console.WriteLine(encoded);

                    var response = await RunPutAsync(uName, encoded, eMail);
                    int responseString = int.Parse(response);
                    if(responseString == 1)
                    {
                        message.SetTitle("Done");
                        message.SetMessage("The password has been changed..");
                        message.SetNegativeButton("OK", (c, ev) => 
                        {
                            Intent intent = new Intent(this, typeof(MainActivity));
                            this.StartActivity(intent);
                        });
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
            };

        }

        public async Task<Person> RunGetAsync(string name)
        {
            try
            {
                var RestUrl = "http://10.20.1.68:45455/api/Person/" + name;

                var request = new HttpRequestMessage(HttpMethod.Get, RestUrl);

                var p = new Person();

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    p = JsonConvert.DeserializeObject<Person>(content);
                }
                return p;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<string> RunPutAsync(string name, string pword, string email)
        {

            try
            {
                HttpClient client = new HttpClient();
                var RestUrl = "http://10.20.1.68:45455/api/Person/" + name;
                var request = new HttpRequestMessage(HttpMethod.Put, RestUrl);

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