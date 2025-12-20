function toggleChatbot() {
    document.getElementById("chatbot-box").classList.toggle("hidden");
}

function showMessage(sender, text) {
    const messages = document.getElementById("chatbot-messages");

    const row = document.createElement("div");
    row.className = `chatbot-row ${sender === "user" ? "from-user" : "from-bot"}`;

    const avatar = document.createElement("div");
    avatar.className = "chatbot-avatar";
    avatar.innerText = sender === "user" ? "Bạn" : "BC";

    const bubble = document.createElement("div");
    bubble.className = "chatbot-bubble";

    if (sender === "bot")
        bubble.innerHTML = `<div class="markdown-body">${marked.parse(text)}</div>`;
    else
        bubble.innerText = text;

    if (sender === "user") {
        row.appendChild(bubble);
        row.appendChild(avatar);
    } else {
        row.appendChild(avatar);
        row.appendChild(bubble);
    }

    messages.appendChild(row);
    messages.scrollTop = messages.scrollHeight;
}

document.getElementById("chatbot-send").onclick = async () => {
    const input = document.getElementById("chatbot-input");
    const msg = input.value.trim();
    if (!msg) return;

    showMessage("user", msg);
    input.value = "";

    const res = await fetch("/api/chat/ask", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ message: msg })
    });

    const data = await res.json();
    showMessage("bot", data.reply);
};

document.getElementById("chatbot-input")
    .addEventListener("keydown", e => {
        if (e.key === "Enter") {
            document.getElementById("chatbot-send").click();
        }
    });