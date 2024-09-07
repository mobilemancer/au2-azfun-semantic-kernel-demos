import { HttpClient } from "@aurelia/fetch-client";

export class ChatBot {
  public chat = "";
  public loading = false;

  private httpClient: HttpClient;
  private endPoint = "chatbot";

  private chatTextarea: HTMLTextAreaElement;
  private diffStart;

  constructor() {
    this.httpClient = new HttpClient();

    this.httpClient.configure((config) => {
      config
        .withDefaults({ mode: "cors" })
        .withBaseUrl("http://localhost:7123/api/");
    });
  }

  public handleKeydown(event: KeyboardEvent): void {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      this.sendText();
    }
  }

  public async sendText(): Promise<void> {
    console.log("Sending text...");

    this.loading = true;
    this.httpClient
      .post(this.endPoint, this.getUserInput())
      .then((response) => response.text())
      .then((text) => {
        this.chat += `\n🤖> ${text}\n`;
        this.saveDiff();
        this.scrollToBottom();
        this.loading = false;
      })
      .catch((error) => {console.error(error);
        this.loading = false;
      });

    console.log("Text sent successfully!");
  }

  saveDiff() {
    this.diffStart = this.chat.length;
  }

  getUserInput() {
    return this.chat.substring(this.diffStart);
  }

  private scrollToBottom(): void {
    if (this.chatTextarea) {
      setTimeout(() => {
        this.chatTextarea.scrollTop = this.chatTextarea.scrollHeight;
      }, 500);
    }
  }
}
