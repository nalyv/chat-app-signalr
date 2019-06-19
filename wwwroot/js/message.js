"use strict";

var connection = new signalR.HubConnectionBuilder()
                        .withUrl("/messages")
                        .build();

var CurrentUserName = '';

connection.on("ReceiveMessage", function(username ,message) {
    if(this.CurrentUserName == username) {//outgoing
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    
        var message;

        message = "<div class='outgoing_msg'><div class='sent_msg'>";
        message += "<p>" + msg + "</p></div></div>";
        
        document.getElementById("messages").innerHTML += message;
    } else { //incoming
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        

        
        var message;
        

        message = "<div class='incoming_msg'><div class='incoming_msg_img'>";
        message += "  <img src='https://ptetutorials.com/images/user-profile.png' alt='sunil'><p>"+ username +"</p> </div>";
        message += "<div class='received_msg'><div class='received_withd_msg'><p>" + msg + "<p>";
        message += "</div></div></div>"; 

        document.getElementById("messages").innerHTML += message;
    }
    
});

connection.on("UserConnnectName", function(username){
    this.CurrentUserName = username;
});

connection.on("UserConnected", function(connectionId){
    document.getElementById("users").innerHTML = "";
    var nameList;
    connectionId.forEach(function(item) {
        nameList = "<div class='chat_list active_chat'><div class='chat_people'>";
        nameList += "<div class='chat_img'> <img src='https://ptetutorials.com/images/user-profile.png' alt='sunil'>";
        nameList += "</div><div class='chat_ib'><h5>" + item + "</h5></div></div></div>";
        document.getElementById("users").innerHTML += nameList;
    });
});

connection.on("UserDisconnected", function(connectionId) {
    var groupElement = document.getElementById("group");
    for(var i = 0; i< groupElement.length; i++){
        if(groupElement.options[i].value == connectionId){
            groupElement.remove(i);
        }
    }
});

connection.start().catch(function(err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function(event) {
    var message = document.getElementById("message").value;
    console.log(message);

    if(!message.indexOf("pm: ")) {

        message = message.replace("pm: ",'');
        var user = message.substring(0,message.indexOf(" "));
        message = message.replace(user+" ",'');

        connection.invoke("SendMessageToUser", user, message).catch(function (err) {
            return console.error(err.toString());
        });

    } else {
        connection.invoke("SendMessageToAll", user, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    
    event.preventDefault();
});

document.getElementById("joinGroup").addEventListener("click", function(event) {
    connection.invoke("JoinGroup", "PrivateGroup").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
