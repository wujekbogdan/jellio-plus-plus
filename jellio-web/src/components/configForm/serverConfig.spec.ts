import { describe, expect, it } from 'vitest';
import { toServerConfig } from './serverConfig';

describe('toServerConfig', () => {
  it('should send the settings the way the config page always did', () => {
    const serverConfig = toServerConfig({
      serverName: 'srv',
      libraries: [
        {
          key: 'aabbccdd-1122-3344-eeff-00112233aabb',
          name: 'Movies',
          type: 'movies',
        },
      ],
      jellyseerrEnabled: true,
      jellyseerrUrl: 'https://jellyseerr.example.com/',
      publicBaseUrl: 'https://jellyfin.example.com//',
    });

    expect(serverConfig).toMatchObject({
      jellyseerrEnabled: true,
      jellyseerrUrl: 'https://jellyseerr.example.com',
      jellyseerrApiKey: '',
      publicBaseUrl: 'https://jellyfin.example.com',
      selectedLibraries: ['aabbccdd11223344eeff00112233aabb'],
    });
  });

  it('should send the sort order chosen for every library, also for disabled ones', () => {
    const serverConfig = toServerConfig({
      serverName: 'srv',
      libraries: [],
      sortOrders: {
        aabbccdd11223344eeff00112233aabb: 'ReleaseDate',
        '11223344556677889900aabbccddeeff': 'Name',
      },
    });

    expect(serverConfig.catalogSortOrders).toEqual([
      {
        libraryId: 'aabbccdd11223344eeff00112233aabb',
        sortOrder: 'ReleaseDate',
      },
      { libraryId: '11223344556677889900aabbccddeeff', sortOrder: 'Name' },
    ]);
  });
});
