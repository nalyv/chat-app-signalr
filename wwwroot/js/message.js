"use strict";

var connection = new signalR.HubConnectionBuilder()
                        .withUrl("/messages")
                        .build();

var CurrentUserName = '';
var SelectedUserName = document.URL.split("/");
SelectedUserName = SelectedUserName[SelectedUserName.length-1];

var unReadMessages = [];

connection.on("ReceiveMessage", function(senderName, receiverName ,message) {

    if(this.CurrentUserName == senderName) {//outgoing
       
        document.getElementById("messages").innerHTML += 
        "<div class='outgoing_msg'>" +
            "<div class='sent_msg'>" +
                "<p>" + message + "</p>" +
            "</div>" +
        "</div>";
    } 
    else if(SelectedUserName === receiverName || 
        (this.CurrentUserName == receiverName && SelectedUserName == senderName)) { //incoming
        document.getElementById("messages").innerHTML += 
        "<div class='incoming_msg'>" +
            "<div class='incoming_msg_img'>"+
                "<img src='https://ptetutorials.com/images/user-profile.png'>" +
                "<p>" + senderName + "</p>" + 
            "</div>" +
            "<div class='received_msg'>" +
                "<div class='received_withd_msg'>" +
                    "<p>" + message + "</p>" +
                "</div>" +
            "</div>" + 
        "</div>";
        
        readMessages(senderName, receiverName);
    }
    else { 
        unreadColorizer(senderName);
        unReadMessages.push({senderName, receiverName, message});
    }
});

connection.on("UserConnnectName", function(username){
    this.CurrentUserName = username;

    readMessages(this.CurrentUserName, SelectedUserName);
});

connection.on("UserConnected", function(connectionId){
    connectionId.forEach(function(item) {
        $("#users").children().each(function(id, element) {
            if(element.id == item) {
                $(element).children().children()[0].innerHTML = 
                "<div class='chat_img'>" + 
                    "<img src='https://ptetutorials.com/images/user-profile.png'>" +
                "</div>" +
                '<h5 style="cursor:pointer" name="SelectUser" onclick="SelectUser(\''+item+'\')">' +
                    item +
                "</h5>" +
                "<img src='../../upload/icon/green.png' style='width:10px; height:10px;'>";
            }
        })
    });
});

connection.on("UserDisconnected", function(connectionId) {
    $("#users").children().each(function(id, element) {
        if(element.id == connectionId) {
            $(element).children().children()[0].innerHTML = 
            "<div class='chat_img'>" + 
                "<img src='https://ptetutorials.com/images/user-profile.png'>" +
            "</div>" +
            '<h5 style="cursor:pointer" name="SelectUser" onclick="SelectUser(\''+connectionId+'\')">' +
                connectionId +
            "</h5>" +
            "<img src='../../upload/icon/red.png' style='width:10px; height:10px;'>";
        }
    })
});

connection.start().catch(function(err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function(event) {
    var message = document.getElementById("message").value;
    
    if(SelectedUserName == "All") {
        connection.invoke("SendMessageToAll", SelectedUserName, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        connection.invoke("SendMessageToUser", SelectedUserName, message).catch(function (err) {
            return console.error(err.toString());
        });

        readMessages(SelectedUserName, this.CurrentUserName);
        
        document.getElementById("messages").innerHTML += 
        "<div class='outgoing_msg'>" +
            "<div class='sent_msg'>" +
                "<p>" + message + "</p>" +
            "</div>" +
        "</div>";
    }

    document.getElementById("message").value="";
    event.preventDefault();
});

document.getElementById("joinGroup").addEventListener("click", function(event) {
    connection.invoke("JoinGroup", "PrivateGroup").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function SelectUser(userName) {
    SelectedUserName = userName;
    window.location.href = userName;
};

function readMessages(senderName,receiverName) {
    unreadUnColorizer(senderName);
    connection.invoke("ReadMessages", senderName, receiverName).catch(function(err){
        return console.error(err.toString());
    });
};

function unreadColorizer(senderName) {
    $("#users").children().each(function(id, element) {
        if(element.id == senderName) {
            $(element)[0].style.backgroundColor = "#b96d90";
        }
    });
};

function unreadUnColorizer(senderName) {
    $("#users").children().each(function(id, element) {
        if(element.id == senderName) {
            $(element)[0].style.backgroundColor = "";
        }
    });
};