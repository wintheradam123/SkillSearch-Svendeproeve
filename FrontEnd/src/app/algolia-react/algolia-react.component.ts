import { Component, Input } from '@angular/core';
import { react2angular } from 'react2angular';
import * as angular from 'angular';
import AlgoliaSearchComponent from './AlgoliaSearchComponent.jsx';

@Component({
  selector: 'app-algolia-react',
  template: `
    <algolia-search
      [appId]="appId"
      [apiKey]="apiKey"
      [indexName]="indexName"
    ></algolia-search>
  `,
})
export class AlgoliaReactComponent {
  @Input() appId?: string;
  @Input() apiKey?: string;
  @Input() indexName?: string;

  constructor() {
    angular
      .module('app')
      .component(
        'algoliaSearch',
        react2angular(AlgoliaSearchComponent, ['appId', 'apiKey', 'indexName'])
      );
  }
}
