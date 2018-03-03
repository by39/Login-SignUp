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

            pWord.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
            rePWord.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;

            string name = uName.Text;
            string pin = pWord.Text;
            string rePin = rePWord.Text;
            string em = email.Text;

            
            signUp.Click += delegate
            {
                bool isSame = pin.Equals(rePin);
                int cmpr = pin.CompareTo(rePin);
                Console.WriteLine(isSame);
                Console.WriteLine(cmpr);
                Console.WriteLine(pin);
                Console.WriteLine(rePin);
            };
        }
    }
}