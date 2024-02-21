declare let AppConfig: AppConfig;

interface AppConfig {
    ApiUrl: string;
    ClientUrl: string;
    Auth: {
        Authority: string;
        SignUp: string;
        Domain: string;
        ClientId: string;
        TenantId: string;
        Scopes: string[];
    };
}

type Nullable<T> = T | undefined | null;

type ControlsOf<T extends Record<string, any>> = {
    [K in keyof T]: T[K] extends Array<any>
    ? import('@angular/forms').FormArray<import('@angular/forms').FormGroup<ControlsOf<T[K][0]>>>
    : T[K] extends Record<any, any>
    ? import('@angular/forms').FormGroup<ControlsOf<T[K]>>
    : import('@angular/forms').FormControl<T[K]>;
};

