import { SourceDto } from "./SourceDto";

export interface NewsArticleDto {
    title: string;
    description: string ;
    url: string ;
    source: SourceDto;
    publishedAt: string ;
    content: string ;
    author: string ;
}