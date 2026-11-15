const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

start();

const button = document.getElementById("input_button");

connection.on("ReceiveMessage", (user, message) => {
    const display = document.getElementById("messages");
    display.textContent += `${user}: ${message}\n`;
});

button.addEventListener("click", async () => {
    const user = document.getElementById("user_input").value;
    const message = document.getElementById("message_input").value;
    const result = await connection.invoke("SendMessage", user, message);
});
