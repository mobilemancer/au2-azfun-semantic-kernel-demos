import { HttpClient } from "@aurelia/fetch-client";

export class KernelMemory {
    public chat = "";
    public input = "What's the web page of Manta Corp";
    public loading = false;

    private httpClient: HttpClient;
    private endPoint = "rag";

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
    }

    public clearChatArea(): void {
        this.chat = "";
    }
}
