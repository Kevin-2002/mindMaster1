namespace mindMaster1;

public partial class MainPage : ContentPage
{
    // GLOBAL VARIABLES //

    //declare constants
    const int MAX_COLUMNS = 4, MAX_ROWS = 12;
    const string SAVE_FILE = "saveFile.txt";

    //declare variables
    int _difficulty, _chosenColour, _currentTurn = 0, _totalAmountOfTurns = 0, _numOfFeedbackCorrect = 0;
    //the reason _rowVisable is initialized to 100 is because I set it's value in an if statement later and would like to keep the value != 0 when not first used
    int _rowVisable = 100;
    bool _generatedOnce = false, _gameLoaded = false;

    //declare arrays
    int[] _answer = new int[MAX_COLUMNS];
    int[,] _guess = new int[MAX_COLUMNS, MAX_ROWS];
    int[,] _feedback = new int[MAX_COLUMNS, MAX_ROWS];
    BoxView[] _answerBoxViews = new BoxView[MAX_COLUMNS];
    BoxView[,] _guessGridBoxViews = new BoxView[MAX_COLUMNS, MAX_ROWS];
    BoxView[,] _feedbackGridBoxViews = new BoxView[MAX_COLUMNS, MAX_ROWS];
    BoxView[,] _choseColourGridBoxViews = new BoxView[MAX_COLUMNS, 2];
    Color[] _possibleColours = {Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.SandyBrown, Colors.Pink, Colors.HotPink, Colors.AliceBlue};

    // GLOBAL VARIABLES //

    ///////////////////////////////////////////////////////////////////////

    // MAIN //

    public MainPage()
	{
		InitializeComponent();
	}

    // MAIN //

    ///////////////////////////////////////////////////////////////////////

    // INITIALIZE GAME //

    private void InitializeGame()
    {
        /* before I decided to make GameGrid in MainPage.xaml
        only let rows and columns generate once
        if(_generatedOnce == false)
        {
            //set up grid containing everything to do with the game
            InitializeGameGrid();
        }*/

        //set up answer combination and grid
        initializeAnswerGrid();

        //set up guesses grid
        InitializeGuessesGrid();

        //set up the grid for feedback
        InitializeFeedbackGrid();

        //set up colour selection grid
        initializeColourSelectorGrid();

        //set all guessGrid boxViews isEnabled to false until start is clicked
        allGuessBoxesIsEnabled(false);

        //help game set the game to what it was
        if (_gameLoaded == true)
        {
            removeExcessGuesses();
            reColourTheGrids();
            allGuessBoxesIsEnabled(true);
            BtnCheckGuess.Text = "Check";
        }

        //as we have generated the rows and columns once we should ensure no more rows or columns are created
        _generatedOnce = true;
    }//end method

/*
    private void InitializeGameGrid()
    {
        //make 2*3 grid for entire game
        //add rows
        for (int r = 0; r < 3; r++)
        {
            GameGrid.RowDefinitions.Add(new RowDefinition());
        }

        //add column
        for (int c = 0; c < 2; c++)
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
    }//end method
*/

    public void initializeAnswerGrid()
    {
        if(_generatedOnce == false)
        {
            for (int c = 0; c < MAX_COLUMNS; c++)
                //add row to see the boxviews
                AnswerGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        //make 4 boxviews for each column
        for (int c = 0; c < MAX_COLUMNS; c++)
        {
            //declare boxview
            BoxView boxView = new BoxView();

            //change attributes of the boxView
            boxView.WidthRequest = 20;
            boxView.HeightRequest = 20;
            boxView.CornerRadius = 3;
            boxView.Color = Colors.Black;

            //add boxviews to the answer grid
            AnswerGrid.Children.Add(boxView);

            //set grid.column
            Grid.SetColumn(boxView, c);

            //store in _answerBoxViews[4]
            _answerBoxViews[c] = boxView;
        }

        //if(true) when there is no game loading
        if(_gameLoaded == false)
            //make method for generating randoms and setting them as colours
            initializeAnswers();
    }//end method

    private void initializeAnswers()
    {
        //declare variables
        int compareNumber = 0, duplicate = 0, amountOfNonDuplicates = 0;
        Random randomColour = new Random();

        //run once for each column (4 times)
        for (int c = 0; c < MAX_COLUMNS; c++)
        {
            //generate random from 0 to 7 for each box
            compareNumber = randomColour.Next(0, 8);

            //this will run when there are many randoms stored
            for (int i = 0; i < amountOfNonDuplicates; i++)
            {
                //check the other randoms to see if compareNumber was drawn
                if (_answer[i] == compareNumber)
                    //increment duplicate
                    duplicate++;
            }

            //will run when the randoms are different
            if (duplicate == 0)
            {
                //store in answer
                _answer[c] = compareNumber;

                //increment the amount of non duplicates successfully generated
                amountOfNonDuplicates++;

                // DEBUG CODE //
                //change colour of answer boxes to see if it works
                //_answerBoxViews[c].Color = _possibleColours[_answer[c]];
            }
            //if the compareNumber was the same as any other in the sequence 
            else if (duplicate == 1)
            {
                c--;
            }

            //reset duplicate variable for a new loop as we are generating a new random to check
            duplicate = 0;
        }//for
    }//end method

    private void InitializeGuessesGrid()
    {
        //only generate rows and columns once
        if(_generatedOnce == false)
        {
            //make 4*12 grid for 4 columns for each row
            //and 1 row for each guess
            //add rows
            for (int r = 0; r < MAX_ROWS; r++)
                GuessGrid.RowDefinitions.Add(new RowDefinition());

            //add column
            for (int c = 0; c < MAX_COLUMNS; c++)
                GuessGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }//if

        //populate with boxviews used for guessing (48 = 12*4)
        for (int r = 0; r < MAX_ROWS; r++)
        {
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                //declare boxview
                BoxView boxView = new BoxView();                

                //add the boxview to the guessgrid
                GuessGrid.Children.Add(boxView);

                //change attributes of the boxView
                boxView.WidthRequest = 20;
                boxView.HeightRequest = 20;
                boxView.CornerRadius = 5;
                boxView.Color = Colors.AliceBlue;

                //change grid.row and grid.column of boxView to populate 1 boxview for each segment
                Grid.SetRow(boxView, r);
                Grid.SetColumn(boxView, c);

                //store boxviews into array
                _guessGridBoxViews[c, r] = boxView;
            }//for
        }//for

        //add the gesture recognizers to change colour of each column
        guessGridAddTapGesture();

        //depending on difficulty chosen give them a certain amount of guesses
        //as I could not find or make a solution to removing RowDefinition I will do this by making boxviews invisable
        //if easy leave all 12 guesses

        removeExcessGuesses();
        
    }//end method

    private void removeExcessGuesses()
    {
        //if medium take away 4 guesses
        if (_difficulty == 1)
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < MAX_COLUMNS; c++)
                    _guessGridBoxViews[c, r].IsVisible = false;

        //if hard take away 8 guesses
        if (_difficulty == 2)
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < MAX_COLUMNS; c++)
                    _guessGridBoxViews[c, r].IsVisible = false;
    }

    private void guessGridAddTapGesture()
    {
        //declare tapGesture variables
        TapGestureRecognizer guessBox1 = new TapGestureRecognizer();
        TapGestureRecognizer guessBox2 = new TapGestureRecognizer();
        TapGestureRecognizer guessBox3 = new TapGestureRecognizer();
        TapGestureRecognizer guessBox4 = new TapGestureRecognizer();

        //add the eventHandler methods to tap variables
        guessBox1.Tapped += GuessBox1_Tapped;
        guessBox2.Tapped += GuessBox2_Tapped;
        guessBox3.Tapped += GuessBox3_Tapped;
        guessBox4.Tapped += GuessBox4_Tapped;

        //add the gestures to the appropriate boxviews
        for (int i = 0; i < MAX_ROWS; i++)
        {
            //column 0 rows 1 - 12
            _guessGridBoxViews[0, i].GestureRecognizers.Add(guessBox1);

            //column 1 rows 1 - 12
            _guessGridBoxViews[1, i].GestureRecognizers.Add(guessBox2);

            //column 2 rows 1 - 12
            _guessGridBoxViews[2, i].GestureRecognizers.Add(guessBox3);

            //column 3 rows 1 - 12
            _guessGridBoxViews[3, i].GestureRecognizers.Add(guessBox4);
        }
    }//end method

    private void InitializeFeedbackGrid()
    {
        //only generate rows and columns once
        if (_generatedOnce == false)
        {
            //make 4*12 grid 4 columns per row
            //1 rows per guess
            //add rows
            for (int r = 0; r < MAX_ROWS; r++)
                FeedbackGrid.RowDefinitions.Add(new RowDefinition());

            //add column
            for (int c = 0; c < MAX_COLUMNS; c++)
                FeedbackGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }//if

        //populate with boxviews used for guessing (48 = 12*4)
        for (int r = 0; r < MAX_ROWS; r++)
        {
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                //declare boxview
                BoxView boxView = new BoxView();

                //add the boxview to the guessgrid
                FeedbackGrid.Children.Add(boxView);

                //change attributes of the boxView
                boxView.WidthRequest = 10;
                boxView.HeightRequest = 10;
                boxView.Color = Colors.AliceBlue;

                //change grid.row and grid.column of boxView to populate 1 boxview for each segment
                Grid.SetRow(boxView, r);
                Grid.SetColumn(boxView, c);

                //store feedback boxviews into array[4][12]
                _feedbackGridBoxViews[c, r] = boxView;
            }//for
        }//for
    }//end method

    private void initializeColourSelectorGrid()
    {
        //declare variables
        int i = 0;

        //generate rows and columns only once
        if (_generatedOnce == false)
        {
            //8 boxes, one for every colour (4*2)
            //rows
            for (int r = 0; r < 2; r++)
                ChoseColourGrid.RowDefinitions.Add(new RowDefinition());

            //columns
            for(int c = 0; c < MAX_COLUMNS; c++)
                ChoseColourGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }//if

        //populate with boxviews used for selecting the colour (8 = 2*4)
        for (int r = 0; r < 2; r++)
        {
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                //declare boxview
                BoxView boxView = new BoxView();
                
                //add the boxview to the choseColourGrid
                ChoseColourGrid.Children.Add(boxView);
                
                //change attributes of the boxView
                boxView.WidthRequest = 20;
                boxView.HeightRequest = 20;
                boxView.CornerRadius = 3;
                boxView.Color = Colors.AliceBlue;

                //change grid.row and grid.column of boxView to populate 1 boxview for each segment
                Grid.SetRow(boxView, r);
                Grid.SetColumn(boxView, c);

                //set every box to 1 of each possible color
                boxView.Color = _possibleColours[i];
                i++;

                //add boxviews to the appropriate array
                _choseColourGridBoxViews[c, r] = boxView;
            }//for
        }//for

        //add tap gestures to the boxvies to select their colour
        ColourSelectorGridAddTapGesture();
    }//end method
    
    private void ColourSelectorGridAddTapGesture()
    {
        //add gesture recognizers for each colour
        //declare the tap variable
        TapGestureRecognizer redChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer blueChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer greenChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer yellowChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer purpleChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer sandyBrownChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer pinkChoiceBox = new TapGestureRecognizer();
        TapGestureRecognizer hotPinkChoiceBox = new TapGestureRecognizer();

        //add the eventHandler method
        redChoiceBox.Tapped += Red_Tapped;
        blueChoiceBox.Tapped += Blue_Tapped;
        greenChoiceBox.Tapped += Green_Tapped;
        yellowChoiceBox.Tapped += Yellow_Tapped;
        purpleChoiceBox.Tapped += Purple_Tapped;
        sandyBrownChoiceBox.Tapped += SandyBrown_Tapped;
        pinkChoiceBox.Tapped += Pink_Tapped;
        hotPinkChoiceBox.Tapped += HotPink_Tapped;

        //add tapGesture to appropriate boxviews
        _choseColourGridBoxViews[0, 0].GestureRecognizers.Add(redChoiceBox);
        _choseColourGridBoxViews[1, 0].GestureRecognizers.Add(blueChoiceBox);
        _choseColourGridBoxViews[2, 0].GestureRecognizers.Add(greenChoiceBox);
        _choseColourGridBoxViews[3, 0].GestureRecognizers.Add(yellowChoiceBox);
        _choseColourGridBoxViews[0, 1].GestureRecognizers.Add(purpleChoiceBox);
        _choseColourGridBoxViews[1, 1].GestureRecognizers.Add(sandyBrownChoiceBox);
        _choseColourGridBoxViews[2, 1].GestureRecognizers.Add(pinkChoiceBox);
        _choseColourGridBoxViews[3, 1].GestureRecognizers.Add(hotPinkChoiceBox);
    }//end method

    //reset grid

    void resetGrid()
    {
        //guessGrid 
        //remove all previous boxiews from the guess grid
        AllGuessBoxesIsVisable(false);

        //reset the turn
        _currentTurn = 0;

        //reset the rowVisable to 100
        _rowVisable = 100;

        //reset the number of win condition
        _numOfFeedbackCorrect = 0;

        //reset the checkButton text, isEnabled and colour values
        BtnCheckGuess.Text = "Start";
        BtnCheckGuess.IsEnabled = true;
        BtnCheckGuess.BackgroundColor = Colors.LimeGreen;

        //reset the btnLoadSave isEnabled and colour values
        BtnLoadSave.IsEnabled = true;
        BtnLoadSave.BackgroundColor = Colors.LimeGreen;

        //reset _gameloaded so if they generate a new game after a save game it will initiialize properly
        _gameLoaded = false;
    }//end methed

    //Set Difficulty

    private void BtnEazy_Clicked(object sender, EventArgs e)
    {
        //set _difficulty and amount of turns for eazy
        _difficulty = 0;
        _totalAmountOfTurns = 11;

        //change color of selected button to Yellow and reset the colours of the others
        BtnEazy.BackgroundColor = Colors.Yellow;
        BtnMedium.BackgroundColor = Colors.Purple;
        BtnHard.BackgroundColor = Colors.Purple;

        //enable next page button and change colour to blue
        BtnNextPage.IsEnabled = true;
        BtnNextPage.BackgroundColor = Colors.Blue;
    }//end method

    private void BtnMedium_Clicked(object sender, EventArgs e)
    {
        //set _difficulty and amount of turns for medium
        _difficulty = 1;
        _totalAmountOfTurns = 7;

        //change color of selected button to Yellow and reset the colours of the others
        BtnEazy.BackgroundColor = Colors.Purple;
        BtnMedium.BackgroundColor = Colors.Yellow;
        BtnHard.BackgroundColor = Colors.Purple;

        //enable next page button and change colour to blue
        BtnNextPage.IsEnabled = true;
        BtnNextPage.BackgroundColor = Colors.Blue;
    }//end method

    private void BtnHard_Clicked(object sender, EventArgs e)
    {
        //set _difficulty and amount of turns for hard
        _difficulty = 2;
        _totalAmountOfTurns = 3;

        //change color of selected button to Yellow and reset the colours of the others
        BtnEazy.BackgroundColor = Colors.Purple;
        BtnMedium.BackgroundColor = Colors.Purple;
        BtnHard.BackgroundColor = Colors.Yellow;

        //enable next page button and change colour to blue
        BtnNextPage.IsEnabled = true;
        BtnNextPage.BackgroundColor = Colors.Blue;
    }//end method

    //usefull Property changes methods

    private void allGuessBoxesIsEnabled(bool enableTrueOrFalse)
    {
        //change all guessBoxes isEnabled values to whatever is passed into the method call
        for (int r = 0; r < MAX_ROWS; r++)
            for (int c = 0; c < MAX_COLUMNS; c++)
                    _guessGridBoxViews[c, r].IsEnabled = enableTrueOrFalse;
    }//end method

    private void AllGuessBoxesIsVisable(bool enableTrueOrFalse)
    {
        //disable every row except the row used in the current turn
        for (int r = 0; r < MAX_ROWS; r++)
            for (int c = 0; c < MAX_COLUMNS; c++)
                _guessGridBoxViews[c, r].IsVisible = enableTrueOrFalse;
    }//end method

    // INITIALIZE GAME //

    //////////////////////////////////////////////////////////////////////

    // GAMEPLAY METHODS //


    private int convertColourToInt(BoxView boxView)
    {
        //  0             1           2               3               4               5               6           7             8
        //Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.SandyBrown, Colors.Pink, Colors.HotPink, colors.AliceBlue};

        //declare variables
        int colourNumber = 0;

        if (boxView.Color == Colors.Red)
            colourNumber = 0;

        else if (boxView.Color == Colors.Blue)
            colourNumber = 1;

        else if (boxView.Color == Colors.Green)
            colourNumber = 2;

        else if (boxView.Color == Colors.Yellow)
            colourNumber = 3;

        else if (boxView.Color == Colors.Purple)
            colourNumber = 4;

        else if (boxView.Color == Colors.SandyBrown)
            colourNumber = 5;

        else if (boxView.Color == Colors.Pink)
            colourNumber = 6;

        else if (boxView.Color == Colors.HotPink)
            colourNumber = 7;
        //in the case that the colour remains unchanged
        else
            colourNumber = 8;

        //return the colours converted number
        return colourNumber;
    }//end method

    private void CheckGuess_Clicked(object sender, EventArgs e)
    {
        //if (false) when this the first time this button is clicked AND when this when the game is in the process of ending
        if (_rowVisable != 100 && _rowVisable != -1)
            //ensure guess is valid
            validateGuessEntry();

        //actually check the guess now that guess is valid
        CheckGuess();
    }//end method

    private void validateGuessEntry()
    {
        //declare variables
        int amountOfNonDuplicates = 0;
        bool validity = true;

        //run 4 times for all columns
        for(int c = 0; c < MAX_COLUMNS; c++)
        {
            //this will run when there are many successfull entries verified
            for (int i = 0; i < amountOfNonDuplicates; i++)
                //check the other guessboxes to see if they share the same colour OR the box still has default colour
                if (_guessGridBoxViews[i, _rowVisable].Color == _guessGridBoxViews[c, _rowVisable].Color || _guessGridBoxViews[c, _rowVisable].Color == Colors.AliceBlue)
                    validity = false;

            //when the guess isn't valid
            if (validity == false)
            {
                //reset grid colours
                for (int x = 0; x < MAX_COLUMNS; x++)
                    _guessGridBoxViews[x, _rowVisable].Color = Colors.AliceBlue;

                //tell the user what happened and give istructions
                DisplayAlert("Error: guess input invalid", "Remove duplicates and blank spaces", "OK");

                //decrement turn as this one isn't valid
                _currentTurn--;
                
                //exit the for loop
                break;
            }//if
            //when the guess is valid increment amountOfNonDuplicates
            else
                amountOfNonDuplicates++;
        }//for
    }//end method

    private void CheckGuess()
    {
        //get feedback from another method
        //if(false) when this the first time this button is clicked AND when this when the game is in the process of ending
        if (_rowVisable != 100 && _rowVisable != -1)
        {
            //store guess colours into integer array
            for (int c = 0; c < MAX_COLUMNS; c++)
                _guess[c, _rowVisable] = convertColourToInt(_guessGridBoxViews[c, _rowVisable]);

            //change the colour of the feedback boxes
            getFeedback();

            //store feedback colours into integer array
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                _feedback[c, _rowVisable] = convertColourToInt(_feedbackGridBoxViews[c, _rowVisable]);
            }

            //enable save button
            BtnSaveGame.IsEnabled = true;

            //change colour if save button to suggest to user it is now available
            BtnSaveGame.BackgroundColor = Colors.LimeGreen; 
        }

        //if(true) when a row is not the last row the difficulty mode should have AND when the guessboxes are not all correct
        //to stop from making another guess available after the game
        if (_rowVisable != (11 - _totalAmountOfTurns) && _numOfFeedbackCorrect != 4)
            makeTurnRowVisable();
        else
            //end the game
            _rowVisable = -1;

        //enable the boxes now that only one row is visable
        allGuessBoxesIsEnabled(true);

        //if(true) when all answers are correct
        if (_numOfFeedbackCorrect == 4)
            endGame(true);

        //if there are no guesses left
        else if (_rowVisable == -1)
            endGame(false);

        //increment what turn it is
        _currentTurn++;
    }

    //to change what row is visable based on the turn
    private void makeTurnRowVisable()
    {
        //change rowVisable so it counts down the turns from 11 to 0
        //as we will use this variable to print the row closest to the bottom of the grid then work our way up to the top
        //if easy
        if (_difficulty == 0)
            _rowVisable = _totalAmountOfTurns - _currentTurn;
        //if medium
        else if (_difficulty == 1)
            //since there are 4 less turns add 4 so it still displays row 11
            _rowVisable = (_totalAmountOfTurns - _currentTurn) + 4;
        //if hard
        else if (_difficulty == 2)
            //since there are 8 less turns add 8 so it still displays row 11
            _rowVisable = (_totalAmountOfTurns - _currentTurn) + 8;
        
        //make rows all invisable on turn 0 only
        if(_currentTurn == 0)
            AllGuessBoxesIsVisable(false);

        //make the row for this turn visable
        for (int c = 0; c < MAX_COLUMNS; c++)
            _guessGridBoxViews[c, _rowVisable].IsVisible = true;

        //change button text from start to check as the appropriate boxvies are now visable
        BtnCheckGuess.Text = "Check";
    }//end method

    private void getFeedback()
    {
        //reset the number of correct answers to reuse the variable each turn
        _numOfFeedbackCorrect = 0;

        //nested for to compare every .color value on answerGrid to every .color value on guessGrid's visable row
        for (int ansCol = 0; ansCol < MAX_COLUMNS; ansCol++)
        {
            for(int guessCol = 0; guessCol < MAX_COLUMNS; guessCol++)
            {
                //access colour comparison by converting colour to int because the _answerBoxViews will need to be black
                //if(true) when colour is correct AND when it is in the right spot
                if (_answer[ansCol] == convertColourToInt(_guessGridBoxViews[guessCol, _rowVisable]) && ansCol == guessCol)
                {
                    //make feedback boxview green
                    _feedbackGridBoxViews[guessCol, _rowVisable].Color = Colors.Green;
                    
                    //store the amount correct to check for the game ending
                    _numOfFeedbackCorrect++;
                }
                //if(true) when colour is correct AND it is the in wrong spot
                else if (_answer[ansCol] == convertColourToInt(_guessGridBoxViews[guessCol, _rowVisable]) && ansCol != guessCol)
                {
                    //make feedback boxview yellow
                    _feedbackGridBoxViews[guessCol, _rowVisable].Color = Colors.Yellow;
                }//else if
            }//for
        }//for
    }//end method

    private void endGame(bool winOrLoss)
    {
        //disable the check button and the save button
        BtnCheckGuess.IsEnabled = false;
        BtnSaveGame.IsEnabled = false;

        //Change the buttons colour to gray to suggest unavailability
        BtnCheckGuess.BackgroundColor = Colors.Gray;
        BtnSaveGame.BackgroundColor = Colors.Gray;

        //display alert saying win or loss
        if(winOrLoss == false)
            DisplayAlert("You lost", "you used all your turns, without cracking the code!", "OK");
        else
            DisplayAlert("You won", "You got the right code!", "OK");

        //display the answers
        for(int c = 0; c < MAX_COLUMNS; c++)
            _answerBoxViews[c].Color = _possibleColours[_answer[c]];
    }//end method


    //implement TapGestureRecognizers 

    private void GuessBox1_Tapped(object sender, EventArgs e)
    {
        //do not run if the game is over
        if(_rowVisable != -1)
            //perform code to change colour of the visable boxview in column 1
            _guessGridBoxViews[0, _rowVisable].Color = _possibleColours[_chosenColour];
    }//end method

    private void GuessBox2_Tapped(object sender, EventArgs e)
    {
        //do not run if the game is over
        if (_rowVisable != -1)
            //perform code to change colour of the visable boxview in column 2
            _guessGridBoxViews[1, _rowVisable].Color = _possibleColours[_chosenColour];
    }//end method

    private void GuessBox3_Tapped(object sender, EventArgs e)
    {
        //do not run if the game is over
        if (_rowVisable != -1)
            //perform code to change colour of the visable boxview in column 3
            _guessGridBoxViews[2, _rowVisable].Color = _possibleColours[_chosenColour];
    }//end method

    private void GuessBox4_Tapped(object sender, EventArgs e)
    {
        //do not run if the game is over
        if (_rowVisable != -1)
            //perform code to change colour of the visable boxview in column 4
            _guessGridBoxViews[3, _rowVisable].Color = _possibleColours[_chosenColour];
    }//end method

    private void Red_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for red through _possibleColours[_chosenColour]
        _chosenColour = 0;
    }//end method

    private void Blue_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for blue through _possibleColours[_chosenColour]
        _chosenColour = 1;
    }//end method

    private void Green_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for green through _possibleColours[_chosenColour]
        _chosenColour = 2;
    }//end method

    private void Yellow_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for yellow through _possibleColours[_chosenColour]
        _chosenColour = 3;
    }//end method

    private void Purple_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for purple through _possibleColours[_chosenColour]
        _chosenColour = 4;
    }//end method

    private void SandyBrown_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for brown through _possibleColours[_chosenColour]
        _chosenColour = 5;
    }//end method

    private void Pink_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for pink through _possibleColours[_chosenColour]
        _chosenColour = 6;
    }//end method

    private void HotPink_Tapped(object sender, EventArgs e)
    {
        //change chosen colour for access for hot pink through _possibleColours[_chosenColour]
        _chosenColour = 7;
    }//end method

    // GAMEPLAY METHODS //

    //////////////////////////////////////////////////////////////////////

    // PAGE NAVIGATION //

    private void BtnNextPage_Clicked(object sender, EventArgs e)
    {
        //move to a non event handler method for loadGame()
        nextPage();
    }//end method

    private void nextPage()
    {
        //initialize the game
        InitializeGame();

        //make Page1 invisable
        Page1.IsVisible = false;

        //make Page2 visable
        Page2.IsVisible = true;
    }

    private void BtnPreviousPage_Clicked(object sender, EventArgs e)
    {
        //run method to return grid to normal
        resetGrid();

        //make Page1 invisable
        Page1.IsVisible = true;

        //make Page2 visable
        Page2.IsVisible = false;
    }//end method

    // PAGE NAVIGATION //

    //////////////////////////////////////////////////////////////////////

    // SAVE GAME //

    private void BtnSaveGame_Clicked(object sender, EventArgs e)
    {
        //declare variables
        string fileText = "";
        String StrRowVisable, StrCurrentTurn, StrDifficulty, StrGuess = "", StrAnswers = "", StrFeedback = "";

        //there was a bug where boxes with colour AliceBlue would change to red when loaded
        //this is caused by these boxviews not yet being checked by convertColourToInt() and therefore
        //saves both AliceBlue and Red to 0
        //solution: run convertColourToInt(); and save then save the answers 
        for (int r = 0; r < MAX_ROWS; r++)
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                _guess[c, r] = convertColourToInt(_guessGridBoxViews[c, r]);
                _feedback[c, r] = convertColourToInt(_feedbackGridBoxViews[c, r]);
            }

        //to write to a file get the filePath of the directory we want to save to and save it to a string
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        //then using the filePath and the file name (declared as a constant) save the complete directory to a string
        //combine the directory we want and the file name we are making or saving to by using path class and its method .Combine()
        string filename = Path.Combine(filePath, SAVE_FILE);

        //Convert things we want to save into Strings
        //single variables
        StrRowVisable = _rowVisable.ToString();
        StrCurrentTurn = _currentTurn.ToString();
        StrDifficulty = _difficulty.ToString();

        //Array variables
        for (int r = 0; r < MAX_ROWS; r++)
            for(int c = 0; c < MAX_COLUMNS; c++)
            {
                //saves colour numbers to file
                StrGuess += _guess[c, r].ToString();
                StrFeedback += _feedback[c, r].ToString();
            }

        //only prints 4 columns so it needs a seperate array for tidyness
        for(int c = 0; c < MAX_COLUMNS; c++)
            StrAnswers += _answer[c].ToString();

        //add everything to fileText and format it (the . is to recognise the end of the string for loading recognition)
        fileText = StrRowVisable + "\n" + StrCurrentTurn + "\n" + StrDifficulty + "\n" + StrGuess + "\n" + StrFeedback + "\n" + StrAnswers + "\n";


        //use streamWriter to print the String to the file in overWrite mode
        using (var w = new StreamWriter(filename, false))
        {
            w.WriteLine(fileText);
        }

        if (fileText == "")
            DisplayAlert("Save Game", "The game has failed to save file", "OK");
        else
            DisplayAlert("Save Game", "The game has successfully loaded to saveFile.txt", "OK");
    }

    // SAVE GAME //

    //////////////////////////////////////////////////////////////////////

    // LOAD GAME //

    private void BtnLoadSave_Clicked(object sender, EventArgs e)
    {
        //declare variables
        int startIndex = 0;
        String StrRowVisable = "", StrCurrentTurn = "", StrDifficulty = "", StrGuess = "", StrAnswers = "", StrFeedback = "";

        //to write to a file get the filePath of the directory we want to save to and save it to a string
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        //then using the filePath and the file name (declared as a constant) save the complete directory to a string
        //combine the directory we want and the file name we are making or saving to by using path class and its method .Combine()
        string filename = Path.Combine(filePath, SAVE_FILE);

        try //read from file and copy into strings
        {
            using (var r = new StreamReader(filename))
            {
                //read in the appropriate line of data to each string
                StrRowVisable = r.ReadLine();
                StrCurrentTurn = r.ReadLine();
                StrDifficulty = r.ReadLine();
                StrGuess = r.ReadLine();
                StrFeedback = r.ReadLine();
                StrAnswers = r.ReadLine();
            }
        

            //now that we have read in the values convert and store them into their integer counterparts
        
                _rowVisable = Convert.ToInt32(StrRowVisable);
                _currentTurn = Convert.ToInt32(StrCurrentTurn);
                _difficulty = Convert.ToInt32(StrDifficulty);
        

            for (int r = 0; r < MAX_ROWS; r++)
                for(int c = 0; c < MAX_COLUMNS; c++)
                {
                    //read one character at a time with substring store in arrays
                    _guess[c, r] = Convert.ToInt32(StrGuess.Substring(startIndex, 1));
                    _feedback[c, r] = Convert.ToInt32(StrFeedback.Substring(startIndex, 1));

                    //increment startIndex
                    startIndex++;
                }

            //reset start index
            startIndex = 0;

            //read in answer array
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                //read in answer store in array one character at a time
                _answer[c] = Convert.ToInt32(StrAnswers.Substring(startIndex, 1));
            
                //increment start index
                startIndex++;
            }

            //variable for triggering methods when grid is initializing
            _gameLoaded = true;

            //move to the next page
            nextPage();
        }//try
        catch
        {
            //message
            DisplayAlert("Load Fail", "There was no save file found, generate a new game", "OK");

            //disable btnLoadSave and change it's colour to gray
            BtnLoadSave.IsEnabled = false;
            BtnLoadSave.BackgroundColor = Colors.Gray;

        }//catch
    }//end method

    private void reColourTheGrids()
    {
        for (int r = 0; r < MAX_ROWS; r++)
            for (int c = 0; c < MAX_COLUMNS; c++)
            {
                _guessGridBoxViews[c, r].Color = _possibleColours[_guess[c, r]];
                _feedbackGridBoxViews[c, r].Color = _possibleColours[_feedback[c, r]];
            }
    }

    // LOAD GAME //
}