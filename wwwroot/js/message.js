"use strict";

var connection = new signalR.HubConnectionBuilder()
                        .withUrl("/messages")
                        .build();

var CurrentUserName = '';
var url = document.URL.split("/");
var SelectedTargetName = url[url.length-1];
var SendType = url[url.length-2];

document.getElementById("title").innerHTML = SelectedTargetName;
console.log("SelectedTargetName");
console.log(SelectedTargetName);
console.log("SendType");
console.log(SendType);


var unReadMessages = [];

connection.on("ReceiveMessage", function(type, senderName, receiverName ,message) {
    console.log(senderName);
    console.log(receiverName);
    console.log(message);

    if(CurrentUserName == senderName) {//outgoing
       
        document.getElementById("messages").innerHTML += 
        "<div class='outgoing_msg'>" +
            "<div class='sent_msg'>" +
                "<p>" + message + "</p>" +
            "</div>" +
        "</div>";
    } 
    else if((SelectedTargetName === receiverName && SendType == type) || 
        (CurrentUserName == receiverName && SelectedTargetName == senderName)) { //incoming
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
        if(type == "pm") {
            unreadColorizer("users", senderName);
        }
        else if(type == "group") {
            unreadColorizer("groups", receiverName);
        }
    }
});

connection.on("UserConnnectName", function(username){
    CurrentUserName = username;

    readMessages(CurrentUserName, SelectedTargetName);
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
                "<img src='../../../upload/icon/green.png' style='width:10px; height:10px;'>";
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
            "<img src='../../../upload/icon/red.png' style='width:10px; height:10px;'>";
        }
    })
});

connection.start().catch(function(err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function(event) {
    var message = document.getElementById("message").value;
    
    if(SendType == "everyone") {
        connection.invoke("SendMessageToAll", SelectedTargetName, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else if(SendType == "group") {
        connection.invoke("SendMessageToGroup", SelectedTargetName, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else if(SendType == "pm") {
        connection.invoke("SendMessageToUser", SelectedTargetName, message).catch(function (err) {
            return console.error(err.toString());
        });

        readMessages(SelectedTargetName, CurrentUserName);
        
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

function readMessages(senderName,receiverName) {
    unreadUnColorizer(senderName);
    connection.invoke("ReadMessages", senderName, receiverName).catch(function(err){
        return console.error(err.toString());
    });
};

function unreadColorizer(id, senderName) {
    $("#"+id).children().each(function(id, element) {
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

function JoinGroup(group) {
    setTimeout(function(){ location.reload(); }, 1000);
    connection.invoke("JoinGroup", group, CurrentUserName).catch(function (err) {
        return console.error(err.toString());
    });
};

// ROUTE
function SelectUser(userName) {
    var currURL = window.location.href;
    currURL = currURL.substring(0, currURL.lastIndexOf(SendType));
    window.location.href = currURL + "pm/" + userName;
};

function SelectBroadcast(item) {
    var currURL = window.location.href;
    currURL = currURL.substring(0, currURL.lastIndexOf(SendType));
    window.location.href = currURL + "everyone/" + item;
};

function SelectGroup(groupName) {   
    var currURL = window.location.href;
    currURL = currURL.substring(0, currURL.lastIndexOf(SendType));
    window.location.href = currURL + "group/" + groupName;
};