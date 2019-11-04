using Android.App;
using Android.Database;
using Android.OS;
using Android.Util;
using Android.Widget;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hangman
{
    [Activity(Theme = "@android:style/Theme.Material.Light", Label = "Hangman", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int[] Pictures;

        private ImageView imageView;
        private TextView textView;
        private ListView scoreView;

        private ICursor cursor;

        private Game Game = new Game();
        private String dbFullPath;

        private Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            textView = FindViewById<TextView>(Resource.Id.AnswerView);
            scoreView = FindViewById<ListView>(Resource.Id.ListScoreBoard);


            Pictures = (new List<int>() {
                Resource.Drawable.hangman_0,
                Resource.Drawable.hangman_1,
                Resource.Drawable.hangman_2,
                Resource.Drawable.hangman_3,
                Resource.Drawable.hangman_4,
                Resource.Drawable.hangman_5,
            }).ToArray();


            InitialiseButtons();
            InitialiseImage();
            InitialiseScore();

            var assets = this.Assets;
            using (var sr = new StreamReader(assets.Open("british-english")))
            {
                var words = new List<string>();
                while (!sr.EndOfStream)
                {
                    string word = sr.ReadLine();
                    words.Add(word);
                }
                Game.NewGame(words);
                Draw();
            }
        }

        // add all the buttons to a dictionary.
        private void InitialiseButtons()
        {
            Buttons.Add("a", FindViewById<Button>(Resource.Id.button_a));
            Buttons.Add("b", FindViewById<Button>(Resource.Id.button_b));
            Buttons.Add("c", FindViewById<Button>(Resource.Id.button_c));
            Buttons.Add("d", FindViewById<Button>(Resource.Id.button_d));
            Buttons.Add("e", FindViewById<Button>(Resource.Id.button_e));
            Buttons.Add("f", FindViewById<Button>(Resource.Id.button_f));
            Buttons.Add("g", FindViewById<Button>(Resource.Id.button_g));
            Buttons.Add("h", FindViewById<Button>(Resource.Id.button_h));
            Buttons.Add("i", FindViewById<Button>(Resource.Id.button_i));
            Buttons.Add("j", FindViewById<Button>(Resource.Id.button_j));
            Buttons.Add("k", FindViewById<Button>(Resource.Id.button_k));
            Buttons.Add("l", FindViewById<Button>(Resource.Id.button_l));
            Buttons.Add("m", FindViewById<Button>(Resource.Id.button_m));
            Buttons.Add("n", FindViewById<Button>(Resource.Id.button_n));
            Buttons.Add("o", FindViewById<Button>(Resource.Id.button_o));
            Buttons.Add("p", FindViewById<Button>(Resource.Id.button_p));
            Buttons.Add("q", FindViewById<Button>(Resource.Id.button_q));
            Buttons.Add("r", FindViewById<Button>(Resource.Id.button_r));
            Buttons.Add("s", FindViewById<Button>(Resource.Id.button_s));
            Buttons.Add("t", FindViewById<Button>(Resource.Id.button_t));
            Buttons.Add("u", FindViewById<Button>(Resource.Id.button_u));
            Buttons.Add("v", FindViewById<Button>(Resource.Id.button_v));
            Buttons.Add("w", FindViewById<Button>(Resource.Id.button_w));
            Buttons.Add("x", FindViewById<Button>(Resource.Id.button_x));
            Buttons.Add("y", FindViewById<Button>(Resource.Id.button_y));
            Buttons.Add("z", FindViewById<Button>(Resource.Id.button_z));
        }

        // redraw the answer textView.
        private void Draw()
        {
            var state = Game.CheckState();
            var id = (int)state;

            if (id > (int)Game.State.CONT5) id = (int)Game.State.CONT5;

            var pic = Pictures[id];
            imageView.SetImageResource(pic);

            var buff = "";
            foreach (var chr in Game.PublicWord)
            {
                buff += chr;
                buff += " ";
            }
            buff.Remove(buff.Length - 1);
            textView.Text = buff;

        }

        private void ResetButtons()
        {
            foreach (var but in Buttons)
            {
                but.Value.Enabled = true;
            }
        }

        // below is dilagos for win and lose scenarios
        private void WinDialog()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("You have won!");
            alert.SetMessage("Congratulations!");
            alert.SetPositiveButton("New Game", (senderAlert, args) =>
            {
                Draw();
                ResetButtons();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
        private void LoseDialog(string answer)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("You have lost :(");
            alert.SetMessage("The word was: " + answer);
            alert.SetPositiveButton("New Game", (senderAlert, args) =>
            {
                Draw();
                ResetButtons();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        // This builds the lambda (which is basically the game loop) for all the key presses.
        private void InitialiseImage()
        {

            foreach (var button in Buttons)
            {
                button.Value.Click +=
                    async (sender, ea) =>
                    {
                        Game.Guess(button.Key);
                        Draw();
                        button.Value.Enabled = false;
                        var state = Game.CheckState();

                        if (state == Game.State.WIN)
                        {
                            Draw();

                            using (var db = new ScoreContext(dbFullPath))
                            {
                                db.Scores.Add(new Score() { Word = Game.Answer, Value = Game.Score() });
                                await db.SaveChangesAsync();
                            }

                            WinDialog();
                            DrawScores();
                            Game.NewGame();
                        }

                        else if (state == Game.State.LOSS)
                        {
                            LoseDialog(Game.Answer);
                            Game.NewGame();
                            Draw();
                        }

                        else
                        {
                            Draw();
                        }
                    };
            }
        }

        //This handles the Entity database setup.
        private async void InitialiseScore()
        {
            var dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var fileName = "scores.db";
            dbFullPath = System.IO.Path.Combine(dbFolder, fileName);

            try
            {
                using (var db = new ScoreContext(dbFullPath))
                {
                    await db.Database.MigrateAsync();
                    await db.SaveChangesAsync();
                }

                DrawScores();
            }

            catch (Exception ex)
            {

                Log.Info("Hangman", ex.ToString());
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }


        }

        // this one redraws the Score board
        private void DrawScores()
        {
            string[] scores;
            using (var db = new ScoreContext(dbFullPath))
            {
                scores = db.Scores.Select(a => $"{a.Word} - {a.Value}").ToArray();
                scoreView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, scores);
            }
        }
    }

}
