using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.LinkLabel;

namespace Wordle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Word Wordy = new Word();
        WordList WordFile = new WordList();
        string chosenWord;
        int guesses = 0;
        double wins = 0;
        double losses = 0;
        double winrate;



        class Word
        {
            public string ValidateWord(string guess, string answer)
            {
                string key = "";
                int i = 0;
                //foreach iterates over all characters in guess and builds a key string to represent the validity of each char
                foreach (char ch in guess)
                {
                    //O is for correct
                    if (answer[i] == ch)
                    {
                        key += "O";
                        i++;
                    }
                    //X is for wrong
                    else if (answer[i] != ch && !answer.Contains(ch))
                    {
                        key += "X";
                        i++;
                    }
                    //Y is for if letter is in word but wrong place
                    else if (answer.Contains(ch))
                    {
                        key += "Y";
                        i++;
                    }
                }
                return key;
            }
        }

        class WordList
        {
            //wordArray will contain only the words
            string[] wordArray = File.ReadAllLines("wordlist.txt");
            //fileArray contains the words and ,0/1's so they can be written back to the file
            string[] fileArray = File.ReadAllLines("wordlist.txt");

            //trims word array so that it can word with the CheckWord method
            public void TrimwordArray()
            {
                for (int i = 0; i < wordArray.Length; i++)
                {
                    wordArray[i] = wordArray[i].Remove(wordArray[i].Length - 2);
                }
            }

            public string SelectWord()
            {
                Random rand = new Random();
                int randomNum = rand.Next(wordArray.Length);
                string chosen = wordArray[randomNum];

                //if chosen word ends up being already used, while loop will force it to change
                while (fileArray[randomNum][6] == 1)
                {
                    randomNum = rand.Next(wordArray.Length);
                    chosen = wordArray[randomNum];
                }


                fileArray[randomNum] = chosen + ",1";
                File.WriteAllLines("wordList.txt", fileArray);
                return chosen;
            }

            //checks if the trimmed wordArray contains the user's guess to make sure the word is valid
            public bool CheckWord(string guess)
            {
                bool containsWord = wordArray.Contains(guess);
                return containsWord;
            }

            //method to clear the game's memory
            public void MemClear()
            {
                for (int i = 0; i < fileArray.Length; i++)
                {
                    fileArray[i] += ",0";
                }
                File.WriteAllLines("wordListTest.txt", fileArray);

            }
        }

        //index to be used in the keypress event
        int i = 0;
        public void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //when user types a letter, it is added to the screen and made uppercase
            if (char.IsLetter(e.KeyChar) && i < 5)
            {
                labels[guesses, i].Text = e.KeyChar.ToString().ToUpper();
                i++;
            }
            //when user hits backspace, most recent typed char is deleted and index goes down
            if (e.KeyChar == (char)Keys.Back)
            {
                if (i > 0)
                {
                    i--;
                }
                labels[guesses, i].Text = "";
                
            }
            //when user hits enter, the guess 'word' is built from the labels and gets validated
            if (e.KeyChar == (char)Keys.Enter && i == 5)
            {
                string word = labels[guesses, 0].Text + labels[guesses, 1].Text + labels[guesses, 2].Text + labels[guesses, 3].Text + labels[guesses, 4].Text;
                label32.Text = word;
                 
                if (WordFile.CheckWord(word) == true || word == chosenWord)
                {
                    //get an answer key in the form "XOXOX" to show locations of correct and incorrect words
                    string key = "";
                    key = Wordy.ValidateWord(word, chosenWord);
                    label33.Text = key;

                    //initialize index value for the 
                    int j = 0;

                    //foreach loop iterates over the key and returns green for correct letters
                    foreach (char ch in key)
                    {
                        if (ch == 'O')
                        {
                            labels[guesses, j].BackColor = Color.Green;
                            j++;
                        }
                        else if (ch == 'Y')
                        {
                            labels[guesses, j].BackColor = Color.Yellow;
                            j++;
                        }
                        else if (ch == 'X')
                        {
                            labels[guesses, j].BackColor = Color.Gray;
                            j++;
                        }
                    }
                    //upon guessing the word, wins and winrate are updated and the game is reset
                    if (key == "OOOOO")
                    {
                        MessageBox.Show("Correct.");
                        wins++;
                        winsLabel.Text = wins.ToString();
                        winrate = (wins / (wins + losses));
                        winRateLabel.Text = winrate.ToString("P");
                        Reset();
                        label33.Text = chosenWord;
                        word = "";
                    }

                    //increment guesses at the end of code
                    guesses++;

                    //if the user guessed too many times, the game will end
                    if (guesses == 6)
                    {
                        MessageBox.Show("GAME OVER the word was " + chosenWord);
                        losses++;
                        lossesLabel.Text = losses.ToString();
                        winrate = (wins / (wins + losses));
                        winRateLabel.Text = winrate.ToString("P");
                        Reset();
                        guesses++;
                        label33.Text = chosenWord;
                        word = "";
                    }

                    //initialize i for the next enter press
                    i = 0;
                }
                else
                {
                    MessageBox.Show("This is not a valid word.");
                }



            }


        }

        //array to ID all labels
        Label[,] labels = new Label[6, 5];

        public void Form1_Load(object sender, EventArgs e)
        {
            //create a 2D array to hold all labels to streamline typing
            labels[0, 0] = label1;
            labels[0, 1] = label2;
            labels[0, 2] = label3;
            labels[0, 3] = label4;
            labels[0, 4] = label5;
            labels[1, 0] = label7;
            labels[1, 1] = label8;
            labels[1, 2] = label9;
            labels[1, 3] = label10;
            labels[1, 4] = label11;
            labels[2, 0] = label12;
            labels[2, 1] = label13;
            labels[2, 2] = label14;
            labels[2, 3] = label15;
            labels[2, 4] = label16;
            labels[3, 0] = label17;
            labels[3, 1] = label18;
            labels[3, 2] = label19;
            labels[3, 3] = label20;
            labels[3, 4] = label21;
            labels[4, 0] = label22;
            labels[4, 1] = label23;
            labels[4, 2] = label24;
            labels[4, 3] = label25;
            labels[4, 4] = label26;
            labels[5, 0] = label6;
            labels[5, 1] = label27;
            labels[5, 2] = label28;
            labels[5, 3] = label29;
            labels[5, 4] = label30;

            WordFile.TrimwordArray();
            chosenWord = WordFile.SelectWord();
            label33.Text = chosenWord;
        }

        //method to functionally reset the game
        public void Reset()
        {
            guesses = -1;

            chosenWord = WordFile.SelectWord();

            foreach (Label label in labels)
            {
                label.Text = "";
                label.BackColor = Color.White;

            }
        }


        private void useWordButton_Click(object sender, EventArgs e)
        {
            bool isTestWordValid = true;
            string testWord = testTextBox.Text;
            if (testWord.Length == 5)
            {
                foreach (char ch in testWord)
                {
                    if (!char.IsLetter(ch))
                    {
                        isTestWordValid = false;
                    }
                }
            }
            else
            {
                isTestWordValid = false;
            }
            if (isTestWordValid)
            {
                chosenWord = testTextBox.Text.ToUpper();
                label33.Text = chosenWord;
            }
            else
            {
                MessageBox.Show("Test word in incorrect format");
            }
            //allow the game keyboard to read keys again
            this.KeyPreview = true;
            label1.Focus();
        }

        //when the textbox is selected, you can no longer type in the game keyboard
        //KeyPreview is essential because it allows the form to continue receiving inputs
        //while another control has focus, like a button. This was a big problem.
        private void testTextBox_Click(object sender, EventArgs e)
        {
            this.KeyPreview = false;
        }

        //by clicking back on the form, focus leaves a control and the keyboard functions again
        private void Form1_Click(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            label1.Focus();
        }

        private void memClearButton_Click(object sender, EventArgs e)
        {
            WordFile.MemClear();
            this.KeyPreview = true;
            label1.Focus();
        }


        private void loadNewWordButton_Click_1(object sender, EventArgs e)
        {
            Reset();
            guesses++;
            label33.Text = chosenWord;
            this.KeyPreview = true;
            label1.Focus();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            foreach (Label label in labels)
            {
                label.Text = "";
                label.BackColor = Color.White;

            }
            guesses = 0;
            i = 0;
            this.KeyPreview = true;
            label1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
