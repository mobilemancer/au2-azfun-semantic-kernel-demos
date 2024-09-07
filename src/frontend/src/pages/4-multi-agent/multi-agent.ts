import { HttpClient } from "@aurelia/fetch-client";

export class MultiAgent {
  public chat = "";
  public input = "Who's the greatest band ever - discuss!";
  public loading = false;

  private httpClient: HttpClient;
  private endPoint = "agentchat";

  private chatTextarea: HTMLTextAreaElement;

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
      this.clearChatArea();
      this.sendText();
    }
  }

  public async sendText(): Promise<void> {
    console.log("Sending text...");

    this.loading = true;
    this.chat = "";
    this.httpClient
      .post(this.endPoint, this.input)
      .then((response) => response.text())
      .then((text) => {
        this.chat += `${text}\n`;
        this.loading = false;
      })
      .catch((error) => {
        console.error(error);
        this.loading = false;
      });

    console.log("Text sent successfully!");
  }

  public clearChatArea(): void {
    this.chat = "";
  }
}
