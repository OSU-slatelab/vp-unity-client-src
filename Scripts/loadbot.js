#pragma strict

function Start (){
	Application.ExternalCall("ChangePatientInfo");
}

function SelectPatientBot(botName) {
	ChatSocket.PatientChoice = botName;
	print ("Selecting patient bot...");
}

function SelectPatientHost(IPNumber){
	ChatSocket.Host = IPNumber;
	print ("Selecting patient host...");
}



//function ScoringButtonToggle(scoreBool){
//	ChatSocket.ScoringToggle = scoreBool;
//}
