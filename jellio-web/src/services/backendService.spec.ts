import { describe, expect, it, vi } from 'vitest';
import { getConfigFromServer, getServerInfo } from './backendService';
import { withMockedFetch } from '@/test-utils/withMockedFetch';

vi.mock('@/lib/utils', () => ({
  getBaseUrl: () => 'https://test.example.com/jelliopp',
  getOrCreateDeviceId: () => 'test-device',
}));

describe('getServerInfo', () => {
  it(
    'should reshape Jellyfin library entries to lower-case domain fields',
    withMockedFetch({
      status: 200,
      headers: { 'content-type': 'application/json' },
      body: {
        name: 'Home Jellyfin',
        libraries: [
          {
            Name: 'Movies',
            Id: 'aabbccdd11223344eeff00112233aabb',
            CollectionType: 'movies',
          },
        ],
      },
    })(async () => {
      const info = await getServerInfo({ token: 'tk' });

      expect(info).toEqual({
        serverName: 'Home Jellyfin',
        libraries: [
          {
            name: 'Movies',
            key: 'aabbccdd11223344eeff00112233aabb',
            type: 'movies',
          },
        ],
      });
    }),
  );
});

describe('getConfigFromServer', () => {
  it(
    'should read the catalog sort orders',
    withMockedFetch({
      status: 200,
      headers: { 'content-type': 'application/json' },
      body: {
        jellyseerrEnabled: false,
        selectedLibraries: ['aabbccdd11223344eeff00112233aabb'],
        catalogSortOrders: [
          {
            libraryId: 'aabbccdd11223344eeff00112233aabb',
            sortOrder: 'ReleaseDate',
          },
        ],
      },
    })(async () => {
      const config = await getConfigFromServer({ token: 'tk' });

      expect(config.catalogSortOrders).toEqual([
        {
          libraryId: 'aabbccdd11223344eeff00112233aabb',
          sortOrder: 'ReleaseDate',
        },
      ]);
    }),
  );

  it(
    'should read a config from a plugin without catalog sort orders',
    withMockedFetch({
      status: 200,
      headers: { 'content-type': 'application/json' },
      body: {
        jellyseerrEnabled: true,
        selectedLibraries: ['aabbccdd11223344eeff00112233aabb'],
      },
    })(async () => {
      const config = await getConfigFromServer({ token: 'tk' });

      expect(config).toEqual({
        jellyseerrEnabled: true,
        selectedLibraries: ['aabbccdd11223344eeff00112233aabb'],
      });
    }),
  );
});
