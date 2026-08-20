import { computed, inject, Service } from "@angular/core";
import
  {
    Api,
    LoginUserCommand,
    LoginUserResponse,
    RefreshAccessTokenResponse,
    RegisterUserCommand
  } from "../../api";
import { AuthTokenStore } from "./auth.token.store";
import { Observable, finalize, shareReplay, tap } from "rxjs";
import { UserStore } from "./user/user.store";

@Service()
export class AuthService
{
  private readonly api = inject(Api);
  private readonly authTokenStore = inject(AuthTokenStore);
  private readonly userStore = inject(UserStore);

  private refreshRequest$: Observable<RefreshAccessTokenResponse> | null = null;

  public readonly isAuthenticated = computed(
    () => this.authTokenStore.isAuthenticated()
  );

  public readonly email = computed(
    () => this.userStore.user()?.email
  );

  public registerUser(request: RegisterUserCommand): Observable<void>
  {
    return this.api.registerUser(request);
  }

  public loginUser(request: LoginUserCommand): Observable<LoginUserResponse>
  {
    return this.api.loginUser(request).pipe(
      tap(response => this.authTokenStore.setToken(response.accessToken)),
      tap(response => this.userStore.setUser(response.user))
    );
  }

  public refreshAccessToken(): Observable<RefreshAccessTokenResponse>
  {
    if (this.refreshRequest$)
    {
      return this.refreshRequest$;
    }

    this.refreshRequest$ = this.api.refreshAccessToken().pipe(
      tap(response =>
      {
        this.authTokenStore.setToken(response.accessToken);
        this.userStore.setUser(response.user);
      }),
      finalize(() =>
      {
        this.refreshRequest$ = null;
      }),
      shareReplay({
        bufferSize: 1,
        refCount: false
      })
    );

    return this.refreshRequest$;
  }

  public logoutUser(): void
  {
    this.authTokenStore.removeToken();
    this.userStore.removeUser();
  }

  public deleteUser(): Observable<void>
  {
    return this.api.deleteUser().pipe(
      tap(() => this.logoutUser())
    );
  }
}
