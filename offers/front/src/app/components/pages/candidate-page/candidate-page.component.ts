import { Offer } from './../../../models/offer';
import { Skill } from './../../../models/skill';
import { BehaviorSubject, Subscription, Observable, Subject } from 'rxjs';
import { DataSource, CollectionViewer } from '@angular/cdk/collections';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CandidateService } from 'src/app/providers/candidate.service';
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-candidate-page',
  templateUrl: './candidate-page.component.html',
  styleUrls: ['./candidate-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CandidatePageComponent implements OnInit {
  public readonly dataSource: OfferDataSource;
  private _isLoading: boolean;

  constructor(private _candidateService: CandidateService, private _snackBar: MatSnackBar, private _router: Router) {
    this._isLoading = false;
    this.dataSource = new OfferDataSource(_candidateService);
  }

  public get isLoading(): Observable<boolean> {
    return this.dataSource.isLoading;
  }

  ngOnInit(): void { }
}

class OfferDataSource extends DataSource<Skill> {
  private readonly _limit: number;
  private _lastRange: number;
  private _cachedData: Offer[];
  private _dataStream: BehaviorSubject<Offer[]>;
  public readonly isLoading: BehaviorSubject<boolean>;
  private _subscription: Subscription;

  constructor(private _candidateService: CandidateService) {
    super();
    this._limit = 20;
    this._lastRange = this._limit - 1;
    this._subscription = new Subscription();
    this.isLoading = new BehaviorSubject<boolean>(true);
    this._cachedData = [];
    this._dataStream = new BehaviorSubject<Offer[]>(this._cachedData);
    this._candidateService.fetchOffers().then(offers => {
      this._cachedData.push(...offers);
      this._dataStream.next(this._cachedData);
    }).catch(exception => {
      console.error(exception);
    }).finally(() => this.isLoading.next(false));

  }

  public connect(collectionViewer: CollectionViewer): Observable<Offer[]> {
    this._subscription.add(collectionViewer.viewChange.subscribe(range => {
      if (range.end >= this._lastRange) {
        this._lastRange += this._limit;
        // FETCH DATA - PUSH IN CACHE AND NEXT STREAM
        this._candidateService.fetchOffers((this._lastRange + 1) / this._limit).then(offers => {
          this._cachedData.push(...offers);
          this._dataStream.next(this._cachedData);
        }).catch(exception => {
          console.error(exception);
        }).finally(() => this.isLoading.next(false));
      }
    }));
    return this._dataStream;
  }

  public disconnect(): void {
    this._subscription.unsubscribe();
  }
}
