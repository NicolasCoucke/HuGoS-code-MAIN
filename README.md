# HuGoS - Humans Go Swarming
 
 HuGoS - Humans Go Swarming - is a Unity-based framework for conducting online experiments on human collective behavior. It is especially suited for the experimental scenario's that are of interest to swarm robotics. For more info please read the corresponding journal paper: http://iridia.ulb.ac.be/IridiaTrSeries/link/IridiaTr2020-014.pdf
 
 
 ## Getting started
 
 Running the basic version of HuGoS only requires going through a few steps. 
 
 1. Download the Unity game engine: https://unity3d.com/get-unity/download
 2. Download the HuGoS code and open the project with Unity.
 3. Register for an account on: https://www.photonengine.com/
 4. In your Unity project, go to the Asset Store and get the "PUN 2 - FREE" package.
 5. Link the Unity project to the Photon server
      * On the Photon Engine website go to "dashboard" and click "create a new app"
      * Under "Photon Type" choose "Photon PUN"
      * Once the application is created, click "Manage" and copy the "app-id"
      * Go back to Unity and navigate to Window->Photon Unity Networking->PUN Wizard
      * Click "Local PhotonServerSettings" and paste the app id in the "App Id Realtime" field
 6. Open the script "Launcher" from the "General" folder and look for the following line of code.
       ```
          AccesCodeField.GetComponent<InputField>().text
       ```
 7. Change the assigned value to the password that you will use to log in to game sessions as experimenter
 8. Open the script "DataSender" from the "General" folder and look for the following line of code.
       ```
           string BasicPath = @
       ```
 9. Change the assigned value to the path where you want the behavioral data to be stored.

Now you're all set to run your first experiment

## Making the experiment accessible to participants
To enable participants to participate in your experiment, they should be able to download an instance of the game and connect to the server.
To distribute the game ot participants, you can make a "build" of your game. You can either make a Windows or IOS build that participants can download to their computer. You can also make a WEBGL build to allow participants to access the participant in the browser. Clear instructions for making each kind of build can be found [here](https://www.youtube.com/watch?v=7nxKAtxGSn8&ab_channel=Brackeys). You can integrate the WebGL build in your own website or upload it for free to http://itch.io. Instructions for making your WebGL accessible in the browser using itch.io can be found [here](https://www.youtube.com/watch?v=fNLpZVNDQqc&ab_channel=N3KEN). When these steps are completed, participants can acces the game build by going to the itch.io link that you provided them. Once participants ahve loaded the starting page of your game, they can connect to the server by entering their name or ID number and clicking the connect button (that is, after the experimenter has initiated the game session, see below).

## Running a basic experiment
The version of HuGoS provided here comes with a basic task that requires participants to barricade as much lava as possible from different spills that are randomly appearing in the environment. Without any changes to the code, 4 conditions of this task are supported: the basic condition, the signalling condition, and the stigmergy condition (please see [our paper](http://iridia.ulb.ac.be/IridiaTrSeries/link/IridiaTr2020-014.pdf) for more details). Before starting an experiment, the experimenter should first schedule a session, recruit the desired number of participants, provide them with the link, and request them to log in at the scheduled time. 

A few minutes before the scheduled time, the experimenter can open the experiment session by logging in with their password. Once logged in, the experimenter enters the *game lobby* and sees a *control panel* where they can change the experimental condition, and the desired number of players. From the moment the experiment session is opened, participants can also enter the game lobby. Unlike the experimenter, participants are presented with a statement of informed consent and a pre-game questionnaire. Once a participant has completed the questionnaire, a green "OK" appears next to that player in the experimenter's view. The experimenter can also choose to *kick* a particular player from the experiment session. The experimenter can choose to either start the experiment automatically, when all participants completed the questionnaires, or manually by pressing the "override start" button.

The experiment begins with a passive tutorial, where participants see a video explaining the task from a birds-eye view. Next comes the active part of the tutorial, where participants can control an avatar from a first-person perspective while further instructions are appearing on the screen. The participants keep this first-person perspective for the remainder of the experiment. The experimenter on the other hand, keeps seeing a bird's eye view of the experiment, and can also choose to kick participants during the experiment (for example, when they are inactive for a certain period). In the current implementation, the an experiment session comprises 3 rounds of 5 minutes each. After the third round, participants get redirected to the end lobby, where they complete another set of questionnaires.

## Handling behavioral data
### Recording data to your computer
If you correctly modified the data save path as indicated in step 8 of "Getting started", then the behavioral data of each experiment session should be automatically saved to your specified path. At the beginning of each experiment seesion, i.e., when you log in as experimenter, a new .csv file is created with the current data and time as name. **Do not open this file while the experiment is running**, as this will prevent data from being stored. The first few lines of the file will consist of participant's answers to the questionnaires. If you wish to change how the questionnaire data is stored, you can open the "SaveAnswers" script and modify the line of code that starts with the following. 
```
QuestionString = DateTime.Now.ToString("HH_mm_ss")
```
After the tutorial phase, each player starts sending his individual data (position, rotation, field of view,...) to the experimenter, who in turn saves it to the .csv file. To modify which data each player sends to the experimenter, open the "PlayerManager" script and look for the line of code that starts with the following.
```
Temp_player_data = pingTime.ToString()
```
 At each timestep, the experimenter instance saves both data related to the task (such as score and spill size) as well as the behavioral data received from the players. This is taken care of in the "DataSender" script. In the current implementation, data is stored 10 times per second. If you fish to change this frequency, modify the last number in the following line of code.
```
InvokeRepeating("StoreGameData", 1f, 0.1f);
```

### Parsing the data
The .csv file will contain the saved experiment data as specified above. Depending on your needs, you can import this file in, e.g., Matlab or Python, and write a script that extract the data you want to analyze. This will probably require some nested loops in order to extract, for example, the positional data of each player at each point in time. As an illustration, the figure below shows part of a .csv file saved from the pilot studies reported in [our paper](http://iridia.ulb.ac.be/IridiaTrSeries/link/IridiaTr2020-014.pdf).

### Sending data to a Google Sheet
Saving data locally to the work station is the most straightforward option to store participant data. Alternatively, data can also be directly sent to a Google Sheet. This has the advantage that data could still keep being stored were the experimenter to experience connection issues. With some further modifications, it could also allow for experiments to be run without the presence of an experimenter. However, this approach causes additional bandwidth usages and is a potential point of failure so it is recommended to not use this approach when not strictly necessary.

To enable online data storage, we have to link the Photon Server with a Google Sheet via a *webhook*, for which we can use [Pipedream](https://pipedream.com/). 
Follow the following steps:
1. In the "DataSender" script, use the function "OutputCommondata" to send data to a webhook at each timepoint by including it in the "StoreGameData" function.
2. Make sure the data you want to send is included in the variable "GameArray" in the following line of code.
   ```
   object[] content = new object[] { GameArray };
   ```
3. Create an account on https://pipedream.com/
4. Create a new workflow and copy the example code [provided here](https://pipedream.com/@NicolasCoucke/pilot_game_data-p_7NCbRr)
5. Link your Google account and fill in the Spreadsheet ID of your Google Sheet in the two indicated locations
6. Copy the webhooklink from the top of the workflow (ending with .. .m.pipedream.net)
7. Go to your Photon Dashboard->manage your app->Webhooks and paste your link in the "BaseUrl" field

## Implementing a custom experiment

TBD

### Some basic notions 

### An overview of the scripts

### Easy changes to make

## Further reading
