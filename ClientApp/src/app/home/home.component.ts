import { Component, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.scss"]
})
export class HomeComponent {
  public hackerNewsStories: HackerNewsStory[];

  constructor(
    private http: HttpClient,
    @Inject("BASE_URL") private baseUrl: string
  ) {
    this.get("");
  }

  get(searchTerm: string) {
    this.http
      .get<HackerNewsStory[]>(
        `${this.baseUrl}hackernews?searchTerm=${searchTerm}`
      )
      .subscribe(
        result => {
          this.hackerNewsStories = result;
        },
        error => console.error(error)
      );
  }

  search(event: KeyboardEvent) {
    this.get((event.target as HTMLTextAreaElement).value);
  }

  open(url: string) {
    window.open(url, "_blank");
  }
}

interface HackerNewsStory {
  title: string;
  by: string;
  url: string;
}
