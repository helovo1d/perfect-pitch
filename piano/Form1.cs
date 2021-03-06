﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Media;


using System.Media;
using System.Collections;
using System.IO;
using System.Threading;
using System.Timers;
using System.Runtime.CompilerServices;



using System.Windows;
using Clipboard = System.Windows.Forms.Clipboard;
using System.Diagnostics;

namespace piano
{


    public partial class Form1 : Form
    {
        static System.Media.SoundPlayer pl = new SoundPlayer();





        public Form1()
        {

            InitializeComponent();
            this.KeyPreview = true;
            readPath();
            foreach (Control c in this.groupBox1.Controls)
            {
                if (c.GetType() == typeof(Button))
                {

                    Button b = (Button)c;
                    if (b.Text.Equals("ignore")) continue;

                    b.MouseDown += new MouseEventHandler(button_pressed);
                    b.MouseUp += new MouseEventHandler(button_released);
                    b.MouseLeave += new EventHandler(button_leave);

                    deviceMap.Add(b.Tag.ToString(), null);
                    load(b.Tag.ToString());
                }
                if (c.GetType() == typeof(CheckBox))
                {
                    CheckBox cb = (CheckBox)c;
                    cb.CheckedChanged += new EventHandler(checked_chagned);
                }
            }

            button37_Click(null, null);
            this.listBox1.MouseDoubleClick += new MouseEventHandler(list_double_clicked);




            toolTip1.SetToolTip(this.button43, "Save current score to clipboard");
            toolTip2.SetToolTip(this.checkBox37, "Turn on/off piano key sound");

            toolTip3.SetToolTip(this.button37, "Preset C,D,E of 2,3,4 octave. The default");

            toolTip4.SetToolTip(this.listBox1, "Double click on selected item to replay note from history");

            toolTip5.SetToolTip(this.progressBar1, "Streak of 20 to win the game!");


            toolTip6.SetToolTip(this.button40, "Also shortcut 'space' to next!");

            toolTip7.SetToolTip(this.button41, "Also shortcut 'r' to repeat!");

            System.Threading.Thread.Sleep(500);

            var c1 = new System.Windows.Media.MediaPlayer();
            c1.Open(new System.Uri(Path.GetFullPath(@"res/loading.wav")));


            deviceMap.Add("loading.wav", c1);
            System.Threading.Thread.Sleep(500);
            c1.Play();

        }


        Dictionary<String, Object> deviceMap = new Dictionary<String, Object>();



        private void load(String note_octave)
        {



            var c = new System.Windows.Media.MediaPlayer();
           
            c.Volume = 0.5;
            c.Open(new System.Uri(Path.GetFullPath(getAudioPath(note_octave))));
            deviceMap[note_octave] = c;
            System.Threading.Thread.Sleep(100);




        }
        private void playWindows(String note_octave)
        {

            System.Windows.Media.MediaPlayer mp = (MediaPlayer)deviceMap[note_octave];
            mp.Stop();
            mp.Position = new TimeSpan(0);
            mp.Play();



        }

        private void C_MediaEnded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private void button_leave(Object o, EventArgs e)
        {
            Button b = (Button)o;
            if (b.Tag.ToString().Length == 3) // black key
            {

                b.BackColor = SystemColors.ActiveCaptionText;
            }
            else
            {
                b.BackColor = SystemColors.ButtonHighlight;
            }
        }

        private void button_released(Object o, MouseEventArgs e)
        {

            Button b = (Button)o;
            if (b.Tag.ToString().Length == 3) // black key
            {

                b.BackColor = SystemColors.ActiveCaptionText;
            }
            else
            {
                b.BackColor = SystemColors.ButtonHighlight;
            }

        }

        private String getAudioPath(String note_octave)
        {

            String octave = note_octave.Substring(note_octave.Length - 1);
            String note = note_octave.Substring(0, note_octave.Length - 1);

            String audio_path = path + "/octave" + octave + "/" + note + ".wav";
            return audio_path;
        }
        private void play_basic(String note_octave)
        {

            pl.Stop();



            pl.SoundLocation = getAudioPath(note_octave);

            pl.Play();
        }
        private void play(String note_octave)
        {

            // playNaudio(note_octave);

            //     play_basic(note_octave);

            playWindows(note_octave);
        }

        private void readPath()
        {

            System.IO.StreamReader file = new System.IO.StreamReader(@"config");
            String line = null;
            while ((line = file.ReadLine()) != null)
            {
                path = line;
                if (line.Contains("path"))
                {
                    path = line.Substring(line.IndexOf("=") + 1);
                    path = path.Trim();
                    break;
                }
            }

            file.Close();
        }
        String path = "res/grand_piano";
        // String path = @"notes2-4.01\";
        //  String path = @"res\";
        private void button_pressed(Object o, MouseEventArgs e)
        {

            Button b = (Button)o;
            b.BackColor = SystemColors.ControlDark;


            if (checkBox37.Checked == false)
            {

                // pl.Stop();

                //   pl.SoundLocation = path + b.Tag + ".wav";

                // pl.Play();
                play(b.Tag.ToString());
            }
            if (current == null) return;

            if (current.Tag.ToString() == b.Tag.ToString())
            {


                if (answered == false)
                {
                    label6.Text = "Good Job!";
                    hits++;
                    label4.Text = hits.ToString();
                    streak++;
                    this.progressBar1.Value = streak * 10;


                    NoteAnswer na = new NoteAnswer(cntList++ + " " + "Answer " + b.Tag.ToString() + " correct");
                    na.note = b.Tag.ToString();

                    this.listBox1.Items.Add(na);
                    this.listBox1.TopIndex = this.listBox1.Items.Count - 1;

                    if (streak == 20)
                    {
                        if (MessageBox.Show("Score is " + hits + " of " + total + "\n\n 'OK' to copy to clipboard", "Congratz!", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                        {
                            stopWatch.Stop();
                            saveToClipboard();



                        }
                    }

                }
                else
                {

                    label6.Text = "Correct!      " + b.Tag.ToString();
                }
            }
            else
            {

                label6.Text = "Miss!";
                if (answered == false)
                {
                    NoteAnswer na = new NoteAnswer(cntList++ + " " + "Answer " + b.Tag.ToString() + " missed");
                    na.note = b.Tag.ToString();


                    this.listBox1.Items.Add(na);
                    this.listBox1.TopIndex = this.listBox1.Items.Count - 1;
                    streak -= 5;
                    if (streak < 0) streak = 0;


                    aTimer = new System.Timers.Timer(30);
                    aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    aTimer.SynchronizingObject = this;

                    aTimer.Start();

                }

            }

            answered = true;

        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (progressBar1.Value > streak * 10)
            {

                progressBar1.Value--;

            }
            else
            {

                aTimer.Stop();
            }

        }

        private static System.Timers.Timer aTimer;

        int cntList = 0;
        private void list_double_clicked(object sender, MouseEventArgs e)
        {

            int index = this.listBox1.IndexFromPoint(e.Location);

            if (index != System.Windows.Forms.ListBox.NoMatches)
            {

                NoteAnswer na = (NoteAnswer)this.listBox1.Items[index];

                //  pl.Stop();

                //  pl.SoundLocation = path + na.note + ".wav";

                //   pl.Play();
                play(na.note);


            }
        }

        HashSet<CheckBox> selected = new HashSet<CheckBox>();
        public String getSelected()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (CheckBox cb in selected)
            {

                sb.Append(cb.Tag.ToString() + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            return sb.ToString();
        }
        private void checked_chagned(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            if (cb.Checked)
            {

                selected.Add(cb);

            }
            else
            {

                selected.Remove(cb);

            }


        }
        int total = 0;
        int hits = 0;
        int streak;
        Boolean answered = false;
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void button37_Click(object sender, EventArgs e)
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)c).Checked = false;

                }

            }
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c2":

                        case "d2":

                        case "e2":

                        case "c3":

                        case "d3":

                        case "e3":

                        case "c4":

                        case "d4":

                        case "e4":
                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }


        }
        CheckBox current;
        Boolean freshStart = true;
        private void button40_Click(object sender, EventArgs e)
        {
            if (selected.Count < 1)
            {
                MessageBox.Show("Check few notes to play (Checkboxes)", "notice");
                return;
            }
            answered = false;
            total++;
            label5.Text = total.ToString();

            this.label6.Text = "";

            Random rnd = new Random();

            current = selected.ElementAt(rnd.Next(selected.Count));

 
            play(current.Tag.ToString());


            NoteAnswer na = new NoteAnswer(cntList++ + " " + "Note played");
            na.note = current.Tag.ToString();

            this.listBox1.Items.Add(na);
            this.listBox1.TopIndex = this.listBox1.Items.Count - 1;

            if (freshStart)
            {
                freshStart = false;

               stopWatch = Stopwatch.StartNew();
            }
        }
        Stopwatch stopWatch = null;

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button39_Click(object sender, EventArgs e)
        {

        }

        private void button39_Click_1(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c2":
                        case "d2":
                        case "e2":
                        case "f2":
                        case "g2":
                        case "a2":
                        case "b2":
                        case "c3":
                        case "d3":
                        case "e3":
                        case "f3":
                        case "g3":
                        case "a3":
                        case "b3":
                        case "c4":
                        case "d4":
                        case "e4":
                        case "f4":
                        case "g4":
                        case "a4":
                        case "b4":

                        case "cd2":
                        case "de2":
                        case "fg2":
                        case "ga2":
                        case "ab2":

                        case "cd3":
                        case "de3":
                        case "fg3":
                        case "ga3":
                        case "ab3":

                        case "cd4":
                        case "de4":
                        case "fg4":
                        case "ga4":
                        case "ab4":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c2":
                        case "d2":
                        case "e2":
                        case "f2":
                        case "g2":
                        case "a2":
                        case "b2":
                        case "c3":
                        case "d3":
                        case "e3":
                        case "f3":
                        case "g3":
                        case "a3":
                        case "b3":
                        case "c4":
                        case "d4":
                        case "e4":
                        case "f4":
                        case "g4":
                        case "a4":
                        case "b4":
                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }

        }

        private void button41_Click(object sender, EventArgs e)
        {
            if (current == null) return;

            //  pl.Stop();
            //  pl.SoundLocation = path + current.Tag + ".wav";
            //   pl.Play();
            play(current.Tag.ToString());
        }

        private void button42_Click(object sender, EventArgs e)
        {
            freshStart = true;
            if (stopWatch != null)
            {
                stopWatch.Stop();
                stopWatch = null;
            }
            total = 0;
            hits = 0;
            listBox1.Items.Clear();
            this.listBox1.TopIndex = 0;
            current = null;
            this.progressBar1.Value = 0;
            label6.Text = "";
            label4.Text = "0";
            label5.Text = "0";
            streak = 0;
            cntList = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        class NoteAnswer : Object
        {
            public String note;
            public String answer;
            public NoteAnswer()
            {

            }
            public NoteAnswer(String answer)
            {

                this.answer = answer;
            }

            public override String ToString()
            {

                return answer;
            }
        }

        private void checkBox37_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void saveToClipboard() {

          
            TimeSpan ts = stopWatch.Elapsed;
            System.Windows.Forms.Clipboard.SetText(DateTime.Now.ToShortDateString() + " " + getSelected()  + String.Format(", {0:0.00}", ts.TotalMinutes) + " min, " + hits + "/" + total + String.Format(" {0:0.00}", (double)hits / (double)total));

        }
        private void button43_Click(object sender, EventArgs e)
        {
            saveToClipboard();
        }
        private void uncheckAll()
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)c).Checked = false;

                }

            }

        }
        private void button46_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c2":
                        case "d2":
                        case "e2":
                        case "f2":
                        case "g2":
                        case "a2":
                        case "b2":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void button47_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c3":
                        case "d3":
                        case "e3":
                        case "f3":
                        case "g3":
                        case "a3":
                        case "b3":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c4":
                        case "d4":
                        case "e4":
                        case "f4":
                        case "g4":
                        case "a4":
                        case "b4":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void button49_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c2":
                        case "d2":
                        case "e2":
                        case "f2":
                        case "g2":
                        case "a2":
                        case "b2":

                        case "cd2":
                        case "de2":
                        case "fg2":
                        case "ga2":
                        case "ab2":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void button50_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c3":
                        case "d3":
                        case "e3":
                        case "f3":
                        case "g3":
                        case "a3":
                        case "b3":

                        case "cd3":
                        case "de3":
                        case "fg3":
                        case "ga3":
                        case "ab3":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void button51_Click(object sender, EventArgs e)
        {
            uncheckAll();
            foreach (Control c in groupBox1.Controls)
            {
                if (c.GetType() == typeof(CheckBox))
                {
                    String str = ((CheckBox)c).Tag.ToString();
                    switch (str)
                    {
                        case "c4":
                        case "d4":
                        case "e4":
                        case "f4":
                        case "g4":
                        case "a4":
                        case "b4":

                        case "cd4":
                        case "de4":
                        case "fg4":
                        case "ga4":
                        case "ab4":

                            ((CheckBox)c).Checked = true;
                            break;
                    }

                }

            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {


            e.Handled = true;
        }
        Boolean isDown = false;



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (isDown == false)
            {


                switch (e.KeyCode)
                {
                    case Keys.Space:
                        button40_Click(null, null);
                        break;

                    case Keys.R:
                        button41_Click(null, null);
                        break;
                }


                e.Handled = true;

                isDown = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            isDown = false;
            e.Handled = true;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            foreach (MediaPlayer mp in deviceMap.Values) {

                mp.Volume = ((double)this.trackBar1.Value / (double)10);
            }
        }
    }
}
