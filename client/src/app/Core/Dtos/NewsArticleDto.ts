import { SourceDto } from "./SourceDto";

export interface NewsArticleDto {
    Title: string;
    Description: string ;
    Url: string ;
    Source: SourceDto;
    PublishedAt: string ;
}