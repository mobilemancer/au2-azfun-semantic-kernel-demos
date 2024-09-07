import { HttpClient } from "@aurelia/fetch-client";

export class SimplePrompt {
  public draft: string;
  public response: string;
  public loading = false;

  private httpClient: HttpClient;
  private endPoint = "simpleprompt";

  constructor() {
    this.httpClient = new HttpClient();

    this.httpClient.configure((config) => {
      config
        .withDefaults({ mode: "cors" })
        .withBaseUrl("http://localhost:7123/api/");
    });
  }

  public async sendText(): Promise<void> {
    console.log("Sending text...");

    this.loading = true;
    this.response = "";
    this.httpClient
      .post(this.endPoint, this.draft)
      .then((response) => response.text())
      .then((text) => {
        this.response = text;
        this.loading = false;
      })
      .catch((error) => {
        console.error(error);
        this.loading = false;
      });

    console.log("Text sent successfully!");
  }
}
