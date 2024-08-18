import React from "react";
import { InstantSearch, SearchBox, Hits } from "react-instantsearch-dom";
import algoliasearch from "algoliasearch/lite";

const AlgoliaSearchComponent = ({ appId, apiKey, indexName }) => {
  const searchClient = algoliasearch(appId, apiKey);

  return (
    <InstantSearch searchClient={searchClient} indexName={indexName}>
      <SearchBox />
      <Hits />
    </InstantSearch>
  );
};

export default AlgoliaSearchComponent;
